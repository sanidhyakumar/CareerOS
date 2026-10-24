using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Domain.Models;
using CareerOS.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchService> _logger;

    public SearchService(ApplicationDbContext context, ILogger<SearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        string query,
        string? entityType = null,
        int limit = 30,
        Guid? profileId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<SearchResult>();
        }

        limit = Math.Clamp(limit, 1, 100);
        var ftsQuery = BuildFtsQuery(query);

        IReadOnlyList<SearchResult> results = Array.Empty<SearchResult>();

        // Try FTS5 MATCH query first
        if (!string.IsNullOrWhiteSpace(ftsQuery))
        {
            try
            {
                var ftsResults = await ExecuteFtsQueryAsync(ftsQuery, entityType, limit, cancellationToken);
                if (ftsResults.Count > 0)
                {
                    results = ftsResults;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FTS5 query failed for '{Query}', falling back to LIKE query.", query);
            }
        }

        // Fallback to LIKE query if no FTS results
        if (results.Count == 0)
        {
            results = await ExecuteLikeFallbackQueryAsync(query, entityType, limit, cancellationToken);
        }

        await EnrichResultsWithProfileOwnershipAsync(results, cancellationToken);

        if (profileId.HasValue)
        {
            results = results.Where(r => r.CareerProfileId == profileId.Value).ToList();
        }

        return results;
    }

    private async Task EnrichResultsWithProfileOwnershipAsync(IReadOnlyList<SearchResult> results, CancellationToken cancellationToken)
    {
        if (results == null || results.Count == 0) return;

        try
        {
            var profiles = await _context.CareerProfiles.AsNoTracking().ToListAsync(cancellationToken);
            var primaryProfile = profiles.FirstOrDefault(p => p.IsPrimary) ?? profiles.FirstOrDefault();
            var profileLookup = profiles.ToDictionary(p => p.Id, p => p);

            var entityGuids = results
                .Select(r => Guid.TryParse(r.EntityId, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .Distinct()
                .ToList();

            var docMap = await _context.Documents
                .Where(d => entityGuids.Contains(d.Id) && d.CareerProfileId != null)
                .ToDictionaryAsync(d => d.Id.ToString(), d => d.CareerProfileId!.Value, cancellationToken);

            var appMap = await _context.JobApplications
                .Where(a => entityGuids.Contains(a.Id) && a.CareerProfileId != null)
                .ToDictionaryAsync(a => a.Id.ToString(), a => a.CareerProfileId!.Value, cancellationToken);

            var contactMap = await _context.Contacts
                .Where(c => entityGuids.Contains(c.Id) && c.CareerProfileId != null)
                .ToDictionaryAsync(c => c.Id.ToString(), c => c.CareerProfileId!.Value, cancellationToken);

            var interviewMap = await _context.Interviews
                .Where(i => entityGuids.Contains(i.Id) && i.CareerProfileId != null)
                .ToDictionaryAsync(i => i.Id.ToString(), i => i.CareerProfileId!.Value, cancellationToken);

            var wikiMap = await _context.WikiArticles
                .Where(w => entityGuids.Contains(w.Id) && w.CareerProfileId != null)
                .ToDictionaryAsync(w => w.Id.ToString(), w => w.CareerProfileId!.Value, cancellationToken);

            foreach (var r in results)
            {
                Guid? profileId = null;
                if (docMap.TryGetValue(r.EntityId, out var dpId)) profileId = dpId;
                else if (appMap.TryGetValue(r.EntityId, out var apId)) profileId = apId;
                else if (contactMap.TryGetValue(r.EntityId, out var cpId)) profileId = cpId;
                else if (interviewMap.TryGetValue(r.EntityId, out var ipId)) profileId = ipId;
                else if (wikiMap.TryGetValue(r.EntityId, out var wpId)) profileId = wpId;

                if (profileId.HasValue && profileLookup.TryGetValue(profileId.Value, out var prof))
                {
                    r.CareerProfileId = prof.Id;
                    r.ProfileName = prof.Name;
                    r.ProfileColor = prof.ColorCode;
                }
                else if (primaryProfile != null)
                {
                    r.CareerProfileId = primaryProfile.Id;
                    r.ProfileName = primaryProfile.Name;
                    r.ProfileColor = primaryProfile.ColorCode;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to enrich search results with profile ownership, continuing.");
        }
    }

    public async Task<int> ReindexAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting full re-indexing of SQLite FTS5 search catalog...");

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        // Ensure FTS table exists
        await EnsureFtsTableAsync(connection, cancellationToken);

        // Clear existing entries
        using (var deleteCmd = connection.CreateCommand())
        {
            deleteCmd.CommandText = "DELETE FROM GlobalSearchEntries;";
            await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        int totalCount = 0;
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Index Companies
            var companies = await _context.Companies.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var c in companies)
            {
                await InsertSearchEntryAsync(connection, transaction,
                    c.Id.ToString(),
                    "Company",
                    c.Name,
                    $"{c.Industry ?? "Company"} • {c.Headquarters ?? "Global"}",
                    $"{c.Description} {c.Notes} {c.WebsiteUrl} {c.SponsoredVisas} {c.Pros} {c.Cons}",
                    c.Tags ?? c.Industry ?? "Company",
                    "/crm",
                    cancellationToken);
                totalCount++;
            }

            // 2. Index Jobs
            var jobs = await _context.Jobs.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var j in jobs)
            {
                await InsertSearchEntryAsync(connection, transaction,
                    j.Id.ToString(),
                    "Job",
                    j.Title,
                    $"{j.Company} • {j.Location ?? "Remote"}",
                    $"{j.Description} {j.Notes} {j.TechStack} {j.Seniority} {j.EmploymentType}",
                    $"{j.Seniority},{j.EmploymentType}",
                    "/jobs",
                    cancellationToken);
                totalCount++;
            }

            // 3. Index Job Applications
            var applications = await _context.JobApplications
                .Include(a => a.Job)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            foreach (var a in applications)
            {
                string jobTitle = a.Job?.Title ?? "Position";
                string comp = a.Job?.Company ?? "Company";
                await InsertSearchEntryAsync(connection, transaction,
                    a.Id.ToString(),
                    "Application",
                    $"{jobTitle} @ {comp}",
                    $"Status: {a.Status} • Applied {a.ApplicationDate:MMM dd, yyyy} via {a.Channel}",
                    $"{a.Notes} {a.Channel}",
                    a.Status.ToString(),
                    "/applications",
                    cancellationToken);
                totalCount++;
            }

            // 4. Index Contacts
            var contacts = await _context.Contacts.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var ct in contacts)
            {
                await InsertSearchEntryAsync(connection, transaction,
                    ct.Id.ToString(),
                    "Contact",
                    ct.Name,
                    $"{ct.JobTitle ?? "Professional"} at {ct.Company ?? "Undisclosed"}",
                    $"{ct.Email} {ct.Phone} {ct.Notes} {ct.RelationshipNotes} {ct.Category}",
                    ct.Tags ?? ct.Category.ToString(),
                    "/contacts",
                    cancellationToken);
                totalCount++;
            }

            // 5. Index Documents
            var documents = await _context.Documents
                .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            foreach (var doc in documents)
            {
                string tags = string.Join(",", doc.DocumentTags.Select(dt => dt.Tag.Name));
                await InsertSearchEntryAsync(connection, transaction,
                    doc.Id.ToString(),
                    "Document",
                    doc.OriginalFileName,
                    $"Category: {doc.Category} • {doc.FileExtension} (v{doc.Version})",
                    $"{doc.Description} {doc.IssuingAuthority} {doc.DocumentNumber} {tags}",
                    tags,
                    "/vault",
                    cancellationToken);
                totalCount++;
            }

            // 6. Index Wiki Articles
            var wikiArticles = await _context.WikiArticles.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var wiki in wikiArticles)
            {
                await InsertSearchEntryAsync(connection, transaction,
                    wiki.Id.ToString(),
                    "Wiki",
                    wiki.Title,
                    $"Category: {wiki.Category} • [[{wiki.Slug}]]",
                    $"{wiki.Summary} {wiki.Content} {wiki.Tags}",
                    wiki.Tags ?? wiki.Category.ToString(),
                    "/wiki",
                    cancellationToken);
                totalCount++;
            }

            // 7. Index Interviews
            var interviews = await _context.Interviews
                .Include(i => i.Job)
                .Include(i => i.Company)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            foreach (var iv in interviews)
            {
                string title = !string.IsNullOrWhiteSpace(iv.Title) ? iv.Title : $"{iv.InterviewType} Interview";
                string org = iv.Company?.Name ?? iv.Job?.Company ?? "Company";
                await InsertSearchEntryAsync(connection, transaction,
                    iv.Id.ToString(),
                    "Interview",
                    title,
                    $"{iv.InterviewType} with {org} • {iv.ScheduledAt:MMM dd, yyyy}",
                    $"{iv.InterviewerNames} {iv.QuestionsAsked} {iv.PreparationNotes} {iv.FeedbackNotes} {iv.Location}",
                    iv.InterviewType.ToString(),
                    "/interviews",
                    cancellationToken);
                totalCount++;
            }

            transaction.Commit();
            _logger.LogInformation("Successfully indexed {TotalCount} records into SQLite FTS5.", totalCount);
            return totalCount;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to reindex SQLite FTS5 search catalog.");
            throw;
        }
    }

    public async Task<IReadOnlyDictionary<string, int>> GetSearchStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Company", 0 },
            { "Job", 0 },
            { "Application", 0 },
            { "Contact", 0 },
            { "Document", 0 },
            { "Wiki", 0 },
            { "Interview", 0 }
        };

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await EnsureFtsTableAsync(connection, cancellationToken);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT EntityType, count(*) AS Cnt FROM GlobalSearchEntries GROUP BY EntityType;";
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            string type = reader.GetString(0);
            int count = reader.GetInt32(1);
            stats[type] = count;
        }

        return stats;
    }

    public async Task UpsertIndexItemAsync(
        string entityId,
        string entityType,
        string title,
        string subtitle,
        string content,
        string tags,
        string actionUrl,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await EnsureFtsTableAsync(connection, cancellationToken);

        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM GlobalSearchEntries WHERE EntityId = @entityId;";
        var pId = deleteCmd.CreateParameter();
        pId.ParameterName = "@entityId";
        pId.Value = entityId;
        deleteCmd.Parameters.Add(pId);
        await deleteCmd.ExecuteNonQueryAsync(cancellationToken);

        await InsertSearchEntryAsync(connection, null, entityId, entityType, title, subtitle, content, tags, actionUrl, cancellationToken);
    }

    public async Task DeleteIndexItemAsync(string entityId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await EnsureFtsTableAsync(connection, cancellationToken);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM GlobalSearchEntries WHERE EntityId = @entityId;";
        var pId = cmd.CreateParameter();
        pId.ParameterName = "@entityId";
        pId.Value = entityId;
        cmd.Parameters.Add(pId);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<SearchResult>> ExecuteFtsQueryAsync(
        string ftsQuery,
        string? entityType,
        int limit,
        CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await EnsureFtsTableAsync(connection, cancellationToken);

        using var cmd = connection.CreateCommand();
        string sql = @"
            SELECT EntityId, EntityType, Title, Subtitle, Content, Tags, ActionUrl,
                   bm25(GlobalSearchEntries) AS Score,
                   snippet(GlobalSearchEntries, -1, '<mark>', '</mark>', '...', 16) AS Snippet
            FROM GlobalSearchEntries
            WHERE GlobalSearchEntries MATCH @query ";

        if (!string.IsNullOrWhiteSpace(entityType) && !entityType.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            sql += " AND EntityType = @entityType ";
            var pType = cmd.CreateParameter();
            pType.ParameterName = "@entityType";
            pType.Value = entityType;
            cmd.Parameters.Add(pType);
        }

        sql += " ORDER BY Score LIMIT @limit;";

        var pQuery = cmd.CreateParameter();
        pQuery.ParameterName = "@query";
        pQuery.Value = ftsQuery;
        cmd.Parameters.Add(pQuery);

        var pLimit = cmd.CreateParameter();
        pLimit.ParameterName = "@limit";
        pLimit.Value = limit;
        cmd.Parameters.Add(pLimit);

        cmd.CommandText = sql;

        var list = new List<SearchResult>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(ReadSearchResult(reader));
        }

        return list;
    }

    private async Task<IReadOnlyList<SearchResult>> ExecuteLikeFallbackQueryAsync(
        string rawQuery,
        string? entityType,
        int limit,
        CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await EnsureFtsTableAsync(connection, cancellationToken);

        using var cmd = connection.CreateCommand();
        string sql = @"
            SELECT EntityId, EntityType, Title, Subtitle, Content, Tags, ActionUrl,
                   0.0 AS Score,
                   Content AS Snippet
            FROM GlobalSearchEntries
            WHERE (Title LIKE @pattern OR Subtitle LIKE @pattern OR Content LIKE @pattern OR Tags LIKE @pattern) ";

        if (!string.IsNullOrWhiteSpace(entityType) && !entityType.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            sql += " AND EntityType = @entityType ";
            var pType = cmd.CreateParameter();
            pType.ParameterName = "@entityType";
            pType.Value = entityType;
            cmd.Parameters.Add(pType);
        }

        sql += " LIMIT @limit;";

        var pPattern = cmd.CreateParameter();
        pPattern.ParameterName = "@pattern";
        pPattern.Value = $"%{rawQuery.Trim()}%";
        cmd.Parameters.Add(pPattern);

        var pLimit = cmd.CreateParameter();
        pLimit.ParameterName = "@limit";
        pLimit.Value = limit;
        cmd.Parameters.Add(pLimit);

        cmd.CommandText = sql;

        var list = new List<SearchResult>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var res = ReadSearchResult(reader);
            // Build a naive snippet highlighting match
            res.Snippet = BuildNaiveSnippet(res.Content, rawQuery);
            list.Add(res);
        }

        return list;
    }

    private static SearchResult ReadSearchResult(DbDataReader reader)
    {
        string entityId = reader.IsDBNull(0) ? "" : reader.GetString(0);
        string entityType = reader.IsDBNull(1) ? "" : reader.GetString(1);
        string title = reader.IsDBNull(2) ? "" : reader.GetString(2);
        string subtitle = reader.IsDBNull(3) ? "" : reader.GetString(3);
        string content = reader.IsDBNull(4) ? "" : reader.GetString(4);
        string tags = reader.IsDBNull(5) ? "" : reader.GetString(5);
        string actionUrl = reader.IsDBNull(6) ? "" : reader.GetString(6);
        double score = reader.IsDBNull(7) ? 0.0 : Convert.ToDouble(reader.GetValue(7));
        string snippet = reader.IsDBNull(8) ? "" : reader.GetString(8);

        if (string.IsNullOrWhiteSpace(snippet))
        {
            snippet = content.Length > 150 ? content.Substring(0, 150) + "..." : content;
        }

        return new SearchResult
        {
            EntityId = entityId,
            EntityType = entityType,
            Title = title,
            Subtitle = subtitle,
            Content = content,
            Snippet = snippet,
            ActionUrl = actionUrl,
            Tags = tags,
            Score = Math.Round(score, 3)
        };
    }

    private static string BuildNaiveSnippet(string content, string term)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;
        int idx = content.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx == -1)
        {
            return content.Length > 150 ? content.Substring(0, 150) + "..." : content;
        }

        int start = Math.Max(0, idx - 40);
        int length = Math.Min(content.Length - start, term.Length + 80);
        string excerpt = content.Substring(start, length);
        string highlighted = Regex.Replace(excerpt, Regex.Escape(term), "<mark>$&</mark>", RegexOptions.IgnoreCase);
        return (start > 0 ? "..." : "") + highlighted + (start + length < content.Length ? "..." : "");
    }

    private static string BuildFtsQuery(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var matches = Regex.Matches(input, @"[\w\.\+#\-]+");
        var tokens = new List<string>();
        foreach (Match match in matches)
        {
            var val = match.Value.Trim().Replace("\"", "");
            if (val.Length > 0)
            {
                tokens.Add($"\"{val}\"*");
            }
        }
        return tokens.Count > 0 ? string.Join(" AND ", tokens) : string.Empty;
    }

    private static async Task EnsureFtsTableAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE VIRTUAL TABLE IF NOT EXISTS GlobalSearchEntries USING fts5(
                EntityId UNINDEXED,
                EntityType UNINDEXED,
                Title,
                Subtitle,
                Content,
                Tags,
                ActionUrl UNINDEXED,
                tokenize = 'porter unicode61'
            );";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task InsertSearchEntryAsync(
        DbConnection connection,
        DbTransaction? transaction,
        string entityId,
        string entityType,
        string title,
        string subtitle,
        string content,
        string tags,
        string actionUrl,
        CancellationToken cancellationToken)
    {
        using var cmd = connection.CreateCommand();
        if (transaction != null) cmd.Transaction = transaction;

        cmd.CommandText = @"
            INSERT INTO GlobalSearchEntries (EntityId, EntityType, Title, Subtitle, Content, Tags, ActionUrl)
            VALUES (@entityId, @entityType, @title, @subtitle, @content, @tags, @actionUrl);";

        AddParam(cmd, "@entityId", entityId);
        AddParam(cmd, "@entityType", entityType);
        AddParam(cmd, "@title", title ?? string.Empty);
        AddParam(cmd, "@subtitle", subtitle ?? string.Empty);
        AddParam(cmd, "@content", content ?? string.Empty);
        AddParam(cmd, "@tags", tags ?? string.Empty);
        AddParam(cmd, "@actionUrl", actionUrl ?? string.Empty);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParam(DbCommand cmd, string name, object? value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(param);
    }
}
