using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class WikiArticleLink : BaseEntity
{
    public Guid SourceArticleId { get; set; }
    public WikiArticle SourceArticle { get; set; } = null!;

    public Guid? TargetArticleId { get; set; }
    public WikiArticle? TargetArticle { get; set; }

    public string TargetSlug { get; set; } = string.Empty;
    public string? LinkText { get; set; }

    public WikiArticleLink() { }

    public static WikiArticleLink Create(Guid sourceArticleId, string targetSlug, Guid? targetArticleId = null, string? linkText = null)
    {
        return new WikiArticleLink
        {
            SourceArticleId = sourceArticleId,
            TargetSlug = targetSlug.Trim().ToLowerInvariant(),
            TargetArticleId = targetArticleId,
            LinkText = linkText?.Trim()
        };
    }
}

