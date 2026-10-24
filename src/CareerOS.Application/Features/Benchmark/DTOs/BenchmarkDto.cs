namespace CareerOS.Application.Features.Benchmark.DTOs;

public class SystemHealthDto
{
    public string Status { get; set; } = "Healthy";
    public long DatabaseSizeBytes { get; set; }
    public double DatabaseSizeMb { get; set; }
    public Dictionary<string, int> TableCounts { get; set; } = new();
    public long MemoryWorkingSetBytes { get; set; }
    public double MemoryWorkingSetMb { get; set; }
    public double ProcessUptimeSeconds { get; set; }
    public string DotNetVersion { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
}

public class BenchmarkResultDto
{
    public string TestName { get; set; } = string.Empty;
    public int TargetRecordCount { get; set; }
    public int ActualRecordsProcessed { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public double RecordsPerSecond { get; set; }
    public double QueryLatencyP50Ms { get; set; }
    public double QueryLatencyP95Ms { get; set; }
    public bool Passed { get; set; }
}

public class BenchmarkReportDto
{
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public List<BenchmarkResultDto> Results { get; set; } = new();
    public long TotalElapsedMilliseconds { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool ProductionReady { get; set; } = true;
}

public class RunBenchmarkRequest
{
    public int ApplicationCount { get; set; } = 10000;
    public int DocumentCount { get; set; } = 50000;
    public int SearchRecordCount { get; set; } = 100000;
    public bool CleanupAfterBenchmark { get; set; } = true;
}
