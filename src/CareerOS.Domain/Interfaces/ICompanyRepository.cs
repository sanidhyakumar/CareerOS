using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<IReadOnlyList<Company>> GetAllAsync(
        string? searchTerm = null,
        string? industry = null,
        SponsorshipStatus? sponsorship = null,
        RelocationStatus? relocation = null,
        int? minRating = null,
        bool? isTarget = null,
        CancellationToken cancellationToken = default);

    Task<Company?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<Company?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default);
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

