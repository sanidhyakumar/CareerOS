using System.Text.RegularExpressions;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class WikiRepository : IWikiRepository
{
    private readonly ApplicationDbContext _context;

    public WikiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WikiArticle?> GetByIdAsync(Guid id, bool includeLinks = true, CancellationToken cancellationToken = default)
    {
        if (!includeLinks)
        {
            return await _context.WikiArticles
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        return await _context.WikiArticles
            .Include(a => a.Company)
            .Include(a => a.Job)
            .Include(a => a.Outlinks)
                .ThenInclude(l => l.TargetArticle)
            .Include(a => a.Inlinks)
                .ThenInclude(l => l.SourceArticle)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<WikiArticle?> GetBySlugAsync(string slug, bool includeLinks = true, CancellationToken cancellationToken = default)
    {
        var normalized = slug.ToLowerInvariant().Trim();

        if (!includeLinks)
        {
            return await _context.WikiArticles
                .FirstOrDefaultAsync(a => a.Slug == normalized, cancellationToken);
        }

        return await _context.WikiArticles
            .Include(a => a.Company)
            .Include(a => a.Job)
            .Include(a => a.Outlinks)
                .ThenInclude(l => l.TargetArticle)
            .Include(a => a.Inlinks)
                .ThenInclude(l => l.SourceArticle)
            .FirstOrDefaultAsync(a => a.Slug == normalized, cancellationToken);
    }

    public async Task<IEnumerable<WikiArticle>> GetAllAsync(
        string? searchTerm = null,
        WikiCategory? category = null,
        string? tag = null,
        Guid? companyId = null,
        bool? isStarred = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.WikiArticles
            .Include(a => a.Company)
            .Include(a => a.Job)
            .Include(a => a.Outlinks)
            .Include(a => a.Inlinks)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower().Trim();
            query = query.Where(a =>
                a.Title.ToLower().Contains(term) ||
                a.Content.ToLower().Contains(term) ||
                (a.Summary != null && a.Summary.ToLower().Contains(term)) ||
                (a.Tags != null && a.Tags.ToLower().Contains(term)));
        }

        if (category.HasValue)
        {
            query = query.Where(a => a.Category == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var t = tag.ToLower().Trim();
            query = query.Where(a => a.Tags != null && a.Tags.ToLower().Contains(t));
        }

        if (companyId.HasValue)
        {
            query = query.Where(a => a.CompanyId == companyId.Value);
        }

        if (isStarred.HasValue)
        {
            query = query.Where(a => a.IsStarred == isStarred.Value);
        }

        return await query
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WikiArticle> AddAsync(WikiArticle article, CancellationToken cancellationToken = default)
    {
        await _context.WikiArticles.AddAsync(article, cancellationToken);

        // Resolve any orphan backlinks that were waiting for this slug
        var orphanLinks = await _context.WikiArticleLinks
            .Where(l => l.TargetSlug == article.Slug && l.TargetArticleId == null)
            .ToListAsync(cancellationToken);

        foreach (var link in orphanLinks)
        {
            link.TargetArticleId = article.Id;
        }

        return article;
    }

    public Task UpdateAsync(WikiArticle article, CancellationToken cancellationToken = default)
    {
        _context.WikiArticles.Update(article);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(WikiArticle article, CancellationToken cancellationToken = default)
    {
        // Outlinks will be cascade deleted; for inlinks, clear reference
        var inlinks = await _context.WikiArticleLinks
            .Where(l => l.TargetArticleId == article.Id)
            .ToListAsync(cancellationToken);

        foreach (var inlink in inlinks)
        {
            inlink.TargetArticleId = null;
        }

        _context.WikiArticles.Remove(article);
    }

    public async Task<List<string>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var rawTags = await _context.WikiArticles
            .Where(a => a.Tags != null && a.Tags != "")
            .Select(a => a.Tags!)
            .ToListAsync(cancellationToken);

        return rawTags
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();
    }

    public async Task SyncBacklinksAsync(Guid articleId, string content, CancellationToken cancellationToken = default)
    {
        // 1. Remove existing outlinks from this article
        var existingOutlinks = await _context.WikiArticleLinks
            .Where(l => l.SourceArticleId == articleId)
            .ToListAsync(cancellationToken);

        _context.WikiArticleLinks.RemoveRange(existingOutlinks);

        if (string.IsNullOrWhiteSpace(content)) return;

        // 2. Extract [[wiki-links]] via regex
        var regex = new Regex(@"\[\[(.*?)\]\]", RegexOptions.Compiled);
        var matches = regex.Matches(content);

        var processedSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in matches)
        {
            var rawLink = match.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(rawLink)) continue;

            string targetSlug;
            string? linkText = null;

            if (rawLink.Contains('|'))
            {
                var parts = rawLink.Split('|', 2);
                targetSlug = WikiArticle.GenerateSlug(parts[0].Trim());
                linkText = parts[1].Trim();
            }
            else
            {
                targetSlug = WikiArticle.GenerateSlug(rawLink);
                linkText = rawLink;
            }

            if (string.IsNullOrWhiteSpace(targetSlug) || processedSlugs.Contains(targetSlug))
            {
                continue;
            }

            processedSlugs.Add(targetSlug);

            // Check if target article already exists
            var targetArticle = await _context.WikiArticles
                .FirstOrDefaultAsync(a => a.Slug == targetSlug, cancellationToken);

            var link = WikiArticleLink.Create(
                sourceArticleId: articleId,
                targetSlug: targetSlug,
                targetArticleId: targetArticle?.Id,
                linkText: linkText);

            await _context.WikiArticleLinks.AddAsync(link, cancellationToken);
        }
    }
}
