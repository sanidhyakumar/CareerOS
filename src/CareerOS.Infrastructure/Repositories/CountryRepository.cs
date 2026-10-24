using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;

    public CountryRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Country>> GetAllAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Countries.AsQueryable();
        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }
        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<Country?> GetByIdAsync(Guid id, bool includeRegions = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Countries.AsQueryable();
        if (includeRegions)
        {
            query = query.Include(c => c.Regions);
        }
        return await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _context.Countries.FirstOrDefaultAsync(c => c.Code == normalized, cancellationToken);
    }

    public async Task<Country> AddAsync(Country country, CancellationToken cancellationToken = default)
    {
        await _context.Countries.AddAsync(country, cancellationToken);
        return country;
    }

    public Task UpdateAsync(Country country, CancellationToken cancellationToken = default)
    {
        _context.Countries.Update(country);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.FindAsync([id], cancellationToken);
        if (country != null) _context.Countries.Remove(country);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Countries.CountAsync(cancellationToken);
}

