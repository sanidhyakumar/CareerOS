using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class SponsorshipRepository : ISponsorshipRepository
{
    private readonly ApplicationDbContext _context;

    public SponsorshipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SponsorshipCase?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.SponsorshipCases
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        return await _context.SponsorshipCases
            .Include(s => s.Country)
            .Include(s => s.Company)
            .Include(s => s.Job)
            .Include(s => s.EvidenceList)
                .ThenInclude(e => e.Document)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SponsorshipCase>> GetAllAsync(
        Guid? countryId = null,
        VisaProgramType? programType = null,
        SponsorshipEligibilityStatus? eligibility = null,
        VisaAvailabilityStatus? availability = null,
        Guid? companyId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SponsorshipCases
            .Include(s => s.Country)
            .Include(s => s.Company)
            .Include(s => s.Job)
            .Include(s => s.EvidenceList)
                .ThenInclude(e => e.Document)
            .AsQueryable();

        if (countryId.HasValue)
        {
            query = query.Where(s => s.CountryId == countryId.Value);
        }

        if (programType.HasValue)
        {
            query = query.Where(s => s.ProgramType == programType.Value);
        }

        if (eligibility.HasValue)
        {
            query = query.Where(s => s.Eligibility == eligibility.Value);
        }

        if (availability.HasValue)
        {
            query = query.Where(s => s.Availability == availability.Value);
        }

        if (companyId.HasValue)
        {
            query = query.Where(s => s.CompanyId == companyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower().Trim();
            query = query.Where(s =>
                s.ProgramName.ToLower().Contains(term) ||
                (s.Notes != null && s.Notes.ToLower().Contains(term)) ||
                s.Country.Name.ToLower().Contains(term) ||
                (s.Company != null && s.Company.Name.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SponsorshipCase> AddAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default)
    {
        await _context.SponsorshipCases.AddAsync(sponsorshipCase, cancellationToken);
        return sponsorshipCase;
    }

    public Task UpdateAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default)
    {
        _context.SponsorshipCases.Update(sponsorshipCase);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default)
    {
        _context.SponsorshipCases.Remove(sponsorshipCase);
        return Task.CompletedTask;
    }

    public async Task<SponsorshipEvidence?> GetEvidenceByIdAsync(Guid evidenceId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorshipEvidences
            .Include(e => e.Document)
            .FirstOrDefaultAsync(e => e.Id == evidenceId, cancellationToken);
    }

    public async Task<SponsorshipEvidence> AddEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default)
    {
        await _context.SponsorshipEvidences.AddAsync(evidence, cancellationToken);
        return evidence;
    }

    public Task UpdateEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default)
    {
        _context.SponsorshipEvidences.Update(evidence);
        return Task.CompletedTask;
    }

    public Task DeleteEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default)
    {
        _context.SponsorshipEvidences.Remove(evidence);
        return Task.CompletedTask;
    }
}

