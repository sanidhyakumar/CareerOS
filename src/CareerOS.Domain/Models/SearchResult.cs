namespace CareerOS.Domain.Models;

public class SearchResult
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // Company, Job, Application, Contact, Document, Wiki, Interview
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public double Score { get; set; }
}
