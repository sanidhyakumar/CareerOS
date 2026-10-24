using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Wiki.DTOs;

public class WikiArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public WikiCategory Category { get; set; }
    public string CategoryName => Category.ToString();

    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Tags { get; set; }
    public List<string> TagsList => !string.IsNullOrWhiteSpace(Tags)
        ? Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        : new();

    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public Guid? JobId { get; set; }
    public string? JobTitle { get; set; }

    public bool IsStarred { get; set; }
    public bool IsPinned { get; set; }
    public int ViewCount { get; set; }

    public List<WikiBacklinkDto> Backlinks { get; set; } = new();
    public List<WikiOutlinkDto> Outlinks { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WikiBacklinkDto
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}

public class WikiOutlinkDto
{
    public string TargetSlug { get; set; } = string.Empty;
    public string? TargetTitle { get; set; }
    public Guid? TargetArticleId { get; set; }
    public string? LinkText { get; set; }
}

public class WikiStatsDto
{
    public int TotalArticles { get; set; }
    public int StarStoriesCount { get; set; }
    public int SystemDesignCount { get; set; }
    public int CompanyResearchCount { get; set; }
    public int InterviewNotesCount { get; set; }
    public int StarredCount { get; set; }
    public int TagsCount { get; set; }
}
