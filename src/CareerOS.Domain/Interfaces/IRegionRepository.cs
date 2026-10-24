using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface IRegionRepository
{
    Task<IEnumerable<Region>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Region>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<Region?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Region> AddAsync(Region region, CancellationToken cancellationToken = default);
    Task UpdateAsync(Region region, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

