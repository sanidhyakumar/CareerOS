using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface ICareerProfileRepository
{
    Task<IEnumerable<CareerProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CareerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CareerProfile?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<CareerProfile?> GetPrimaryProfileAsync(CancellationToken cancellationToken = default);
    Task<CareerProfile> AddAsync(CareerProfile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(CareerProfile profile, CancellationToken cancellationToken = default);
    Task DeleteAsync(CareerProfile profile, CancellationToken cancellationToken = default);
    Task SetActiveAsync(Guid id, CancellationToken cancellationToken = default);
}
