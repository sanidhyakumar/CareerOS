using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly ApplicationDbContext _context;

    public RegionRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Region>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Regions.Include(r => r.Country).OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Region>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default)
        => await _context.Regions.Where(r => r.CountryId == countryId).OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public async Task<Region?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Regions.Include(r => r.Country).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Region> AddAsync(Region region, CancellationToken cancellationToken = default)
    {
        await _context.Regions.AddAsync(region, cancellationToken);
        return region;
    }

    public Task UpdateAsync(Region region, CancellationToken cancellationToken = default)
    {
        _context.Regions.Update(region);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var region = await _context.Regions.FindAsync([id], cancellationToken);
        if (region != null) _context.Regions.Remove(region);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Regions.CountAsync(cancellationToken);
}

