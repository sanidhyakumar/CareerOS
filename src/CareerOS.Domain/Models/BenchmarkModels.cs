namespace CareerOS.Domain.Models;

public class BenchmarkMetric
{
    public string Name { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public long ElapsedMs { get; set; }
    public double ThroughputPerSec { get; set; }
    public double LatencyMs { get; set; }
    public bool Passed { get; set; }
}

public class BenchmarkSummary
{
    public DateTime RunAt { get; set; } = DateTime.UtcNow;
    public List<BenchmarkMetric> Metrics { get; set; } = new();
    public long TotalElapsedMs { get; set; }
    public bool IsProductionReady { get; set; } = true;
    public string Message { get; set; } = string.Empty;
}

