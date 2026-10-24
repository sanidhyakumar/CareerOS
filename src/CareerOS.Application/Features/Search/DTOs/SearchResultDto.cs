namespace CareerOS.Application.Features.Search.DTOs;

public class SearchResultDto
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class SearchSummaryDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalMatches { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public Dictionary<string, int> MatchesByType { get; set; } = new();
    public List<SearchResultDto> Results { get; set; } = new();
}

public class SearchStatsDto
{
    public int TotalIndexedRecords { get; set; }
    public Dictionary<string, int> RecordsByType { get; set; } = new();
    public DateTime LastIndexedAt { get; set; } = DateTime.UtcNow;
}
