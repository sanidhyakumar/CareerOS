using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAllAsync(bool activeOnly = false, CancellationToken cancellationToken = default);
    Task<Country?> GetByIdAsync(Guid id, bool includeRegions = false, CancellationToken cancellationToken = default);
    Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Country> AddAsync(Country country, CancellationToken cancellationToken = default);
    Task UpdateAsync(Country country, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

