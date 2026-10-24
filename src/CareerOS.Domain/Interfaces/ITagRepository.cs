using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

