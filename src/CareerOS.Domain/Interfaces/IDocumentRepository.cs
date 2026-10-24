using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetAllAsync(bool latestOnly = true, CancellationToken cancellationToken = default);
    Task<Document?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> GetBySha256Async(string sha256Hash, CancellationToken cancellationToken = default);
    Task<Document> AddAsync(Document document, CancellationToken cancellationToken = default);
    Task UpdateAsync(Document document, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> SearchAsync(string? searchTerm, Guid? countryId = null, Guid? regionId = null, Guid? tagId = null, CancellationToken cancellationToken = default);
}

