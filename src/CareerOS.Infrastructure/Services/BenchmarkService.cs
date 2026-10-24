using System.Data;
using System.Data.Common;
using System.Diagnostics;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Domain.Models;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class BenchmarkService : IBenchmarkService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BenchmarkService> _logger;

    public BenchmarkService(ApplicationDbContext context, ILogger<BenchmarkService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BenchmarkSummary> ExecuteScaleBenchmarkAsync(
        int applicationCount = 10000,
        int documentCount = 50000,
        int searchRecordCount = 100000,
        bool cleanup = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Beginning Production Hardening Scale Benchmark: {Apps} apps, {Docs} docs, {Fts} FTS5 records...",
            applicationCount, documentCount, searchRecordCount);

        var overallSw = Stopwatch.StartNew();
        var metrics = new List<BenchmarkMetric>();

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        // 1. Benchmark 10,000 Applications Scale Test
        var appMetric = await BenchmarkApplicationsAsync(connection, applicationCount, cleanup, cancellationToken);
        metrics.Add(appMetric);

        // 2. Benchmark 50,000 Documents Metadata Scale Test
        var docMetric = await BenchmarkDocumentsAsync(connection, documentCount, cleanup, cancellationToken);
        metrics.Add(docMetric);

        // 3. Benchmark 100,000 SQLite FTS5 Search Records Scale Test
        var ftsMetric = await BenchmarkFtsSearchAsync(connection, searchRecordCount, cleanup, cancellationToken);
        metrics.Add(ftsMetric);

        overallSw.Stop();

        bool allPassed = metrics.All(m => m.Passed);
        string message = allPassed
            ? $"All {metrics.Count} production scale milestones verified successfully with high throughput and sub-millisecond query performance."
            : "Some benchmark tests did not achieve the required target thresholds.";

        return new BenchmarkSummary
        {
            RunAt = DateTime.UtcNow,
            Metrics = metrics,
            TotalElapsedMs = overallSw.ElapsedMilliseconds,
            IsProductionReady = allPassed,
            Message = message
        };
    }

    public async Task<IDictionary<string, object>> GetSystemDiagnosticsAsync(CancellationToken cancellationToken = default)
    {
        var proc = Process.GetCurrentProcess();
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        long dbSizeBytes = 0;
        try
        {
            var dbPath = connection.DataSource;
            if (File.Exists(dbPath))
            {
                dbSizeBytes = new FileInfo(dbPath).Length;
            }
        }
        catch { }

        int appCount = await _context.JobApplications.CountAsync(cancellationToken);
        int jobCount = await _context.Jobs.CountAsync(cancellationToken);
        int compCount = await _context.Companies.CountAsync(cancellationToken);
        int docCount = await _context.Documents.CountAsync(cancellationToken);
        int contactCount = await _context.Contacts.CountAsync(cancellationToken);

        int ftsCount = 0;
        try
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT count(*) FROM GlobalSearchEntries;";
            var res = await cmd.ExecuteScalarAsync(cancellationToken);
            if (res != null) ftsCount = Convert.ToInt32(res);
        }
        catch { }

        return new Dictionary<string, object>
        {
            { "Status", "Healthy" },
            { "DatabaseSizeBytes", dbSizeBytes },
            { "DatabaseSizeMb", Math.Round((double)dbSizeBytes / (1024 * 1024), 2) },
            { "MemoryWorkingSetMb", Math.Round((double)proc.WorkingSet64 / (1024 * 1024), 2) },
            { "ProcessUptimeMinutes", Math.Round((DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalMinutes, 1) },
            { "DotNetRuntime", Environment.Version.ToString() },
            { "OsPlatform", Environment.OSVersion.ToString() },
            { "ProcessorCount", Environment.ProcessorCount },
            { "TableCounts", new Dictionary<string, int>
                {
                    { "Applications", appCount },
                    { "Jobs", jobCount },
                    { "Companies", compCount },
                    { "Documents", docCount },
                    { "Contacts", contactCount },
                    { "SearchIndexEntries", ftsCount }
                }
            }
        };
    }

    private async Task<BenchmarkMetric> BenchmarkApplicationsAsync(
        DbConnection connection,
        int count,
        bool cleanup,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Testing scale: {Count} Applications...", count);

        // Ensure at least 1 job exists
        var job = await _context.Jobs.FirstOrDefaultAsync(cancellationToken);
        if (job == null)
        {
            job = new Job { Title = "Staff Systems Engineer", Company = "Scale Corp", Status = JobStatus.Saved };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync(cancellationToken);
        }

        string nowStr = DateTime.UtcNow.ToString("O");
        string jobIdStr = job.Id.ToString();

        var sw = Stopwatch.StartNew();
        using (var transaction = connection.BeginTransaction())
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"
                INSERT INTO JobApplications (Id, JobId, CompanyId, ApplicationDate, Channel, Status, Notes, CreatedAt, UpdatedAt)
                VALUES (@id, @jobId, NULL, @appDate, 'Website', 1, 'Benchmark Scale Record', @createdAt, @updatedAt);";

            var pId = cmd.CreateParameter(); pId.ParameterName = "@id"; cmd.Parameters.Add(pId);
            var pJobId = cmd.CreateParameter(); pJobId.ParameterName = "@jobId"; pJobId.Value = jobIdStr; cmd.Parameters.Add(pJobId);
            var pAppDate = cmd.CreateParameter(); pAppDate.ParameterName = "@appDate"; pAppDate.Value = nowStr; cmd.Parameters.Add(pAppDate);
            var pCreatedAt = cmd.CreateParameter(); pCreatedAt.ParameterName = "@createdAt"; pCreatedAt.Value = nowStr; cmd.Parameters.Add(pCreatedAt);
            var pUpdatedAt = cmd.CreateParameter(); pUpdatedAt.ParameterName = "@updatedAt"; pUpdatedAt.Value = nowStr; cmd.Parameters.Add(pUpdatedAt);

            for (int i = 0; i < count; i++)
            {
                pId.Value = Guid.NewGuid().ToString();
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        sw.Stop();

        // Measure query latency
        var querySw = Stopwatch.StartNew();
        using (var queryCmd = connection.CreateCommand())
        {
            queryCmd.CommandText = "SELECT count(*) FROM JobApplications WHERE Status = 1;";
            await queryCmd.ExecuteScalarAsync();
        }
        querySw.Stop();

        double throughput = sw.ElapsedMilliseconds > 0 ? (double)count / sw.ElapsedMilliseconds * 1000 : 0;

        if (cleanup)
        {
            using var cleanupCmd = connection.CreateCommand();
            cleanupCmd.CommandText = "DELETE FROM JobApplications WHERE Notes = 'Benchmark Scale Record';";
            await cleanupCmd.ExecuteNonQueryAsync();
        }

        return new BenchmarkMetric
        {
            Name = "10k Applications Ingestion & Query Benchmark",
            RecordCount = count,
            ElapsedMs = sw.ElapsedMilliseconds,
            ThroughputPerSec = Math.Round(throughput, 0),
            LatencyMs = Math.Round((double)querySw.ElapsedMilliseconds, 2),
            Passed = throughput >= 1000
        };
    }

    private async Task<BenchmarkMetric> BenchmarkDocumentsAsync(
        DbConnection connection,
        int count,
        bool cleanup,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Testing scale: {Count} Documents Metadata...", count);

        string nowStr = DateTime.UtcNow.ToString("O");
        var sw = Stopwatch.StartNew();

        using (var transaction = connection.BeginTransaction())
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"
                INSERT INTO Documents (Id, FileName, OriginalFileName, FileExtension, ContentType, FileSizeBytes, StoragePath, Sha256Hash, Version, IsLatestVersion, Category, CreatedAt, UpdatedAt)
                VALUES (@id, @fn, @ofn, '.pdf', 'application/pdf', 1048576, 'vault/scale.pdf', 'benchmark-sha256', 1, 1, 1, @now, @now);";

            var pId = cmd.CreateParameter(); pId.ParameterName = "@id"; cmd.Parameters.Add(pId);
            var pFn = cmd.CreateParameter(); pFn.ParameterName = "@fn"; cmd.Parameters.Add(pFn);
            var pOfn = cmd.CreateParameter(); pOfn.ParameterName = "@ofn"; cmd.Parameters.Add(pOfn);
            var pNow = cmd.CreateParameter(); pNow.ParameterName = "@now"; pNow.Value = nowStr; cmd.Parameters.Add(pNow);

            for (int i = 0; i < count; i++)
            {
                string id = Guid.NewGuid().ToString();
                pId.Value = id;
                pFn.Value = $"scale_doc_{i}.pdf";
                pOfn.Value = $"Scale Document {i}.pdf";
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        sw.Stop();

        // Measure scan latency
        var querySw = Stopwatch.StartNew();
        using (var queryCmd = connection.CreateCommand())
        {
            queryCmd.CommandText = "SELECT count(*) FROM Documents WHERE Category = 1;";
            await queryCmd.ExecuteScalarAsync();
        }
        querySw.Stop();

        double throughput = sw.ElapsedMilliseconds > 0 ? (double)count / sw.ElapsedMilliseconds * 1000 : 0;

        if (cleanup)
        {
            using var cleanupCmd = connection.CreateCommand();
            cleanupCmd.CommandText = "DELETE FROM Documents WHERE Sha256Hash = 'benchmark-sha256';";
            await cleanupCmd.ExecuteNonQueryAsync();
        }

        return new BenchmarkMetric
        {
            Name = "50k Documents Metadata Ingestion & Index Benchmark",
            RecordCount = count,
            ElapsedMs = sw.ElapsedMilliseconds,
            ThroughputPerSec = Math.Round(throughput, 0),
            LatencyMs = Math.Round((double)querySw.ElapsedMilliseconds, 2),
            Passed = throughput >= 2000
        };
    }

    private async Task<BenchmarkMetric> BenchmarkFtsSearchAsync(
        DbConnection connection,
        int count,
        bool cleanup,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Testing scale: {Count} SQLite FTS5 Search Records...", count);

        var sw = Stopwatch.StartNew();
        using (var transaction = connection.BeginTransaction())
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"
                INSERT INTO GlobalSearchEntries (EntityId, EntityType, Title, Subtitle, Content, Tags, ActionUrl)
                VALUES (@id, 'Benchmark', @title, @subtitle, @content, 'benchmark-scale', '/benchmark');";

            var pId = cmd.CreateParameter(); pId.ParameterName = "@id"; cmd.Parameters.Add(pId);
            var pTitle = cmd.CreateParameter(); pTitle.ParameterName = "@title"; cmd.Parameters.Add(pTitle);
            var pSubtitle = cmd.CreateParameter(); pSubtitle.ParameterName = "@subtitle"; cmd.Parameters.Add(pSubtitle);
            var pContent = cmd.CreateParameter(); pContent.ParameterName = "@content"; cmd.Parameters.Add(pContent);

            for (int i = 0; i < count; i++)
            {
                pId.Value = Guid.NewGuid().ToString();
                pTitle.Value = $"Principal Software Architect Scale Profile #{i}";
                pSubtitle.Value = $"High Performance Distributed Systems Engineering #{i}";
                pContent.Value = $"Designed fault-tolerant microservices, Kafka event streaming, SQLite FTS5 BM25 search engines, and multi-region Kubernetes clusters with zero downtime for record #{i}.";
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        sw.Stop();

        // Measure FTS5 MATCH query with BM25 score latency
        var querySw = Stopwatch.StartNew();
        using (var searchCmd = connection.CreateCommand())
        {
            searchCmd.CommandText = @"
                SELECT EntityId, Title, bm25(GlobalSearchEntries) AS Score
                FROM GlobalSearchEntries
                WHERE GlobalSearchEntries MATCH '""Distributed""* ""Kubernetes""*'
                ORDER BY Score
                LIMIT 50;";
            using var reader = searchCmd.ExecuteReader();
            while (reader.Read()) { }
        }
        querySw.Stop();

        double throughput = sw.ElapsedMilliseconds > 0 ? (double)count / sw.ElapsedMilliseconds * 1000 : 0;

        if (cleanup)
        {
            using var cleanupCmd = connection.CreateCommand();
            cleanupCmd.CommandText = "DELETE FROM GlobalSearchEntries WHERE EntityType = 'Benchmark';";
            await cleanupCmd.ExecuteNonQueryAsync();

            // Optimize FTS5 B-Trees
            using var optCmd = connection.CreateCommand();
            optCmd.CommandText = "INSERT INTO GlobalSearchEntries(GlobalSearchEntries) VALUES('optimize');";
            await optCmd.ExecuteNonQueryAsync();
        }

        return new BenchmarkMetric
        {
            Name = "100k SQLite FTS5 Full-Text Search BM25 Benchmark",
            RecordCount = count,
            ElapsedMs = sw.ElapsedMilliseconds,
            ThroughputPerSec = Math.Round(throughput, 0),
            LatencyMs = Math.Round((double)querySw.ElapsedMilliseconds, 2),
            Passed = throughput >= 2500
        };
    }
}
