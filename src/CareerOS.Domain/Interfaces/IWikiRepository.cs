using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IWikiRepository
{
    Task<WikiArticle?> GetByIdAsync(Guid id, bool includeLinks = true, CancellationToken cancellationToken = default);
    Task<WikiArticle?> GetBySlugAsync(string slug, bool includeLinks = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiArticle>> GetAllAsync(
        string? searchTerm = null,
        WikiCategory? category = null,
        string? tag = null,
        Guid? companyId = null,
        bool? isStarred = null,
        CancellationToken cancellationToken = default);

    Task<WikiArticle> AddAsync(WikiArticle article, CancellationToken cancellationToken = default);
    Task UpdateAsync(WikiArticle article, CancellationToken cancellationToken = default);
    Task DeleteAsync(WikiArticle article, CancellationToken cancellationToken = default);

    Task<List<string>> GetTagsAsync(CancellationToken cancellationToken = default);
    Task SyncBacklinksAsync(Guid articleId, string content, CancellationToken cancellationToken = default);
}

