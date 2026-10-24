using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface ICredentialRepository
{
    Task<CredentialItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CredentialItem>> GetAllAsync(Guid? companyId = null, CancellationToken cancellationToken = default);
    Task AddAsync(CredentialItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(CredentialItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
