namespace CareerOS.Application.Features.Portability.DTOs;

public class ImportSummaryDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, int> RecordsImported { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class ExportManifestDto
{
    public string ExportVersion { get; set; } = "1.0.0";
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public string AppName { get; set; } = "CareerOS";
    public Dictionary<string, int> RecordCounts { get; set; } = new();
    public string ChecksumSha256 { get; set; } = string.Empty;
}
