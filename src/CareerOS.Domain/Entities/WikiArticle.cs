using System.Text.RegularExpressions;
using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class WikiArticle : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public WikiCategory Category { get; set; } = WikiCategory.General;

    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Tags { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? JobId { get; set; }
    public Job? Job { get; set; }

    public bool IsStarred { get; set; }
    public bool IsPinned { get; set; }
    public int ViewCount { get; set; }

    public ICollection<WikiArticleLink> Outlinks { get; set; } = new List<WikiArticleLink>();
    public ICollection<WikiArticleLink> Inlinks { get; set; } = new List<WikiArticleLink>();

    public WikiArticle() { }

    public static WikiArticle Create(
        string title,
        string content,
        WikiCategory category = WikiCategory.General,
        string? slug = null,
        string? summary = null,
        string? tags = null,
        Guid? companyId = null,
        Guid? jobId = null,
        bool isStarred = false,
        bool isPinned = false)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        string finalSlug = !string.IsNullOrWhiteSpace(slug)
            ? GenerateSlug(slug)
            : GenerateSlug(title);

        return new WikiArticle
        {
            Title = title.Trim(),
            Slug = finalSlug,
            Category = category,
            Content = content ?? string.Empty,
            Summary = summary?.Trim(),
            Tags = tags?.Trim(),
            CompanyId = companyId,
            JobId = jobId,
            IsStarred = isStarred,
            IsPinned = isPinned
        };
    }

    public void Update(
        string title,
        string content,
        WikiCategory category,
        string? slug,
        string? summary,
        string? tags,
        Guid? companyId,
        Guid? jobId,
        bool isStarred,
        bool isPinned)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        Title = title.Trim();
        if (!string.IsNullOrWhiteSpace(slug))
        {
            Slug = GenerateSlug(slug);
        }
        Category = category;
        Content = content ?? string.Empty;
        Summary = summary?.Trim();
        Tags = tags?.Trim();
        CompanyId = companyId;
        JobId = jobId;
        IsStarred = isStarred;
        IsPinned = isPinned;
        UpdatedAt = DateTime.UtcNow;
    }

    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Guid.NewGuid().ToString("n")[..8];
        string slug = text.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("n")[..8] : slug;
    }
}
