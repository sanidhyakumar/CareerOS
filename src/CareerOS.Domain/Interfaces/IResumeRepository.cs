using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface IResumeRepository
{
    Task<IEnumerable<Resume>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Resume?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Resume> AddAsync(Resume resume, CancellationToken cancellationToken = default);
    Task UpdateAsync(Resume resume, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

