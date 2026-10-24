using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Company>> GetAllAsync(
        string? searchTerm = null,
        string? industry = null,
        SponsorshipStatus? sponsorship = null,
        RelocationStatus? relocation = null,
        int? minRating = null,
        bool? isTarget = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Companies
            .Include(c => c.Jobs)
            .Include(c => c.Contacts)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)) ||
                (c.Headquarters != null && c.Headquarters.ToLower().Contains(term)) ||
                (c.Industry != null && c.Industry.ToLower().Contains(term)) ||
                (c.Tags != null && c.Tags.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(industry))
        {
            query = query.Where(c => c.Industry != null && c.Industry.ToLower() == industry.Trim().ToLower());
        }

        if (sponsorship.HasValue)
        {
            query = query.Where(c => c.SponsorshipStatus == sponsorship.Value);
        }

        if (relocation.HasValue)
        {
            query = query.Where(c => c.RelocationStatus == relocation.Value);
        }

        if (minRating.HasValue)
        {
            query = query.Where(c => c.PersonalRating >= minRating.Value);
        }

        if (isTarget.HasValue)
        {
            query = query.Where(c => c.IsTarget == isTarget.Value);
        }

        return await query
            .OrderByDescending(c => c.IsTarget)
            .ThenByDescending(c => c.PersonalRating)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Company?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Companies.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(c => c.Jobs)
                .Include(c => c.Contacts);
        }

        return await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Company?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var trimmed = name.Trim().ToLower();
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmed, cancellationToken);
    }

    public async Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        await _context.Companies.AddAsync(company, cancellationToken);
        return company;
    }

    public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Update(company);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _context.Companies.FindAsync(new object[] { id }, cancellationToken);
        if (company != null)
        {
            _context.Companies.Remove(company);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Companies.CountAsync(cancellationToken);
    }
}

