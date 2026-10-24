using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;

    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetProfileAsync(bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        IQueryable<UserProfile> query = _context.Profiles;

        if (includeDetails)
        {
            query = query
                .Include(p => p.Citizenships)
                    .ThenInclude(c => c.Country)
                .Include(p => p.WorkAuthorizations)
                    .ThenInclude(w => w.Country)
                .Include(p => p.WorkAuthorizations)
                    .ThenInclude(w => w.Document)
                .Include(p => p.SocialLinks.OrderBy(s => s.DisplayOrder))
                .Include(p => p.Addresses)
                    .ThenInclude(a => a.Country)
                .Include(p => p.Addresses)
                    .ThenInclude(a => a.Region);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        IQueryable<UserProfile> query = _context.Profiles.Where(p => p.Id == id);

        if (includeDetails)
        {
            query = query
                .Include(p => p.Citizenships)
                    .ThenInclude(c => c.Country)
                .Include(p => p.WorkAuthorizations)
                    .ThenInclude(w => w.Country)
                .Include(p => p.WorkAuthorizations)
                    .ThenInclude(w => w.Document)
                .Include(p => p.SocialLinks.OrderBy(s => s.DisplayOrder))
                .Include(p => p.Addresses)
                    .ThenInclude(a => a.Country)
                .Include(p => p.Addresses)
                    .ThenInclude(a => a.Region);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfile> AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        await _context.Profiles.AddAsync(profile, cancellationToken);
        return profile;
    }

    public Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        _context.Profiles.Update(profile);
        return Task.CompletedTask;
    }

    public async Task<ProfileCitizenship?> GetCitizenshipByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProfileCitizenships
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddCitizenshipAsync(ProfileCitizenship citizenship, CancellationToken cancellationToken = default)
    {
        await _context.ProfileCitizenships.AddAsync(citizenship, cancellationToken);
    }

    public async Task DeleteCitizenshipAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.ProfileCitizenships.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.ProfileCitizenships.Remove(item);
        }
    }

    public async Task<WorkAuthorization?> GetWorkAuthorizationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkAuthorizations
            .Include(w => w.Country)
            .Include(w => w.Document)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task AddWorkAuthorizationAsync(WorkAuthorization workAuth, CancellationToken cancellationToken = default)
    {
        await _context.WorkAuthorizations.AddAsync(workAuth, cancellationToken);
    }

    public Task UpdateWorkAuthorizationAsync(WorkAuthorization workAuth, CancellationToken cancellationToken = default)
    {
        _context.WorkAuthorizations.Update(workAuth);
        return Task.CompletedTask;
    }

    public async Task DeleteWorkAuthorizationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.WorkAuthorizations.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.WorkAuthorizations.Remove(item);
        }
    }

    public async Task<SocialLink?> GetSocialLinkByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SocialLinks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddSocialLinkAsync(SocialLink socialLink, CancellationToken cancellationToken = default)
    {
        await _context.SocialLinks.AddAsync(socialLink, cancellationToken);
    }

    public Task UpdateSocialLinkAsync(SocialLink socialLink, CancellationToken cancellationToken = default)
    {
        _context.SocialLinks.Update(socialLink);
        return Task.CompletedTask;
    }

    public async Task DeleteSocialLinkAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.SocialLinks.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.SocialLinks.Remove(item);
        }
    }

    public async Task<Address?> GetAddressByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .Include(a => a.Country)
            .Include(a => a.Region)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAddressAsync(Address address, CancellationToken cancellationToken = default)
    {
        await _context.Addresses.AddAsync(address, cancellationToken);
    }

    public Task UpdateAddressAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Update(address);
        return Task.CompletedTask;
    }

    public async Task DeleteAddressAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Addresses.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.Addresses.Remove(item);
        }
    }
}

