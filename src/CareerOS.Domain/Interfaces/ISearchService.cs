using CareerOS.Domain.Models;

namespace CareerOS.Domain.Interfaces;

public interface ISearchService
{
    Task<IReadOnlyList<SearchResult>> SearchAsync(string query, string? entityType = null, int limit = 30, Guid? profileId = null, CancellationToken cancellationToken = default);
    Task<int> ReindexAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, int>> GetSearchStatsAsync(CancellationToken cancellationToken = default);
    Task UpsertIndexItemAsync(string entityId, string entityType, string title, string subtitle, string content, string tags, string actionUrl, CancellationToken cancellationToken = default);
    Task DeleteIndexItemAsync(string entityId, CancellationToken cancellationToken = default);
}

