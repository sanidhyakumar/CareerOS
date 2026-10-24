using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class CareerProfileRepository : ICareerProfileRepository
{
    private readonly ApplicationDbContext _context;

    public CareerProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CareerProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<CareerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.Id == id, cancellationToken);
    }

    public async Task<CareerProfile?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.Slug.ToLower() == slug.ToLower(), cancellationToken);
    }

    public async Task<CareerProfile?> GetPrimaryProfileAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CareerProfiles
            .Include(cp => cp.Preferences)
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.IsPrimary, cancellationToken)
            ?? await _context.CareerProfiles
                .Include(cp => cp.Preferences)
                .Include(cp => cp.Addresses)
                .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CareerProfile> AddAsync(CareerProfile profile, CancellationToken cancellationToken = default)
    {
        await _context.CareerProfiles.AddAsync(profile, cancellationToken);
        return profile;
    }

    public Task UpdateAsync(CareerProfile profile, CancellationToken cancellationToken = default)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        _context.CareerProfiles.Update(profile);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CareerProfile profile, CancellationToken cancellationToken = default)
    {
        _context.CareerProfiles.Remove(profile);
        return Task.CompletedTask;
    }

    public async Task SetActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profiles = await _context.CareerProfiles.ToListAsync(cancellationToken);
        foreach (var p in profiles)
        {
            p.IsActive = (p.Id == id);
            p.UpdatedAt = DateTime.UtcNow;
        }
    }
}
