using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class ResumeCenterRepository : IResumeCenterRepository
{
    private readonly ApplicationDbContext _context;

    public ResumeCenterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Profiles
    public async Task<IReadOnlyList<ResumeProfile>> GetAllProfilesAsync(bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ResumeProfiles
            .Include(p => p.Versions)
            .Include(p => p.Submissions)
            .AsNoTracking();

        if (!includeArchived)
        {
            query = query.Where(p => !p.IsArchived);
        }

        return await query.OrderByDescending(p => p.IsDefault).ThenBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<ResumeProfile?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResumeProfiles
            .Include(p => p.Versions)
                .ThenInclude(v => v.Document)
            .Include(p => p.CoverLetters)
            .Include(p => p.Submissions)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ResumeProfile> AddProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default)
    {
        await _context.ResumeProfiles.AddAsync(profile, cancellationToken);
        return profile;
    }

    public Task UpdateProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default)
    {
        _context.ResumeProfiles.Update(profile);
        return Task.CompletedTask;
    }

    public Task DeleteProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default)
    {
        _context.ResumeProfiles.Remove(profile);
        return Task.CompletedTask;
    }

    public async Task SetDefaultProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profiles = await _context.ResumeProfiles.ToListAsync(cancellationToken);
        foreach (var p in profiles)
        {
            p.IsDefault = (p.Id == profileId);
        }
    }

    // Versions
    public async Task<IReadOnlyList<ResumeVersion>> GetVersionsByProfileIdAsync(Guid profileId, bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ResumeVersions
            .Include(v => v.Document)
            .Include(v => v.ResumeProfile)
            .Include(v => v.Submissions)
            .Where(v => v.ResumeProfileId == profileId);

        if (!includeArchived)
        {
            query = query.Where(v => !v.IsArchived);
        }

        return await query.OrderByDescending(v => v.IsBaseVersion).ThenByDescending(v => v.VersionNumber).ToListAsync(cancellationToken);
    }

    public async Task<ResumeVersion?> GetVersionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResumeVersions
            .Include(v => v.Document)
            .Include(v => v.ResumeProfile)
            .Include(v => v.Submissions)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<ResumeVersion> AddVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default)
    {
        await _context.ResumeVersions.AddAsync(version, cancellationToken);
        return version;
    }

    public Task UpdateVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default)
    {
        _context.ResumeVersions.Update(version);
        return Task.CompletedTask;
    }

    public Task DeleteVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default)
    {
        _context.ResumeVersions.Remove(version);
        return Task.CompletedTask;
    }

    public async Task<int> GetNextVersionNumberAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var maxVersion = await _context.ResumeVersions
            .Where(v => v.ResumeProfileId == profileId)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync(cancellationToken);

        return (maxVersion ?? 0) + 1;
    }

    // Cover Letters
    public async Task<IReadOnlyList<CoverLetter>> GetAllCoverLettersAsync(Guid? profileId = null, bool templatesOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.CoverLetters
            .Include(c => c.ResumeProfile)
            .AsNoTracking();

        if (profileId.HasValue)
        {
            query = query.Where(c => c.ResumeProfileId == profileId.Value || c.IsTemplate);
        }

        if (templatesOnly)
        {
            query = query.Where(c => c.IsTemplate);
        }

        return await query.OrderByDescending(c => c.IsTemplate).ThenBy(c => c.Title).ToListAsync(cancellationToken);
    }

    public async Task<CoverLetter?> GetCoverLetterByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CoverLetters
            .Include(c => c.ResumeProfile)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CoverLetter> AddCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default)
    {
        await _context.CoverLetters.AddAsync(letter, cancellationToken);
        return letter;
    }

    public Task UpdateCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default)
    {
        _context.CoverLetters.Update(letter);
        return Task.CompletedTask;
    }

    public Task DeleteCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default)
    {
        _context.CoverLetters.Remove(letter);
        return Task.CompletedTask;
    }

    // Portfolio Versions
    public async Task<IReadOnlyList<PortfolioVersion>> GetAllPortfolioVersionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PortfolioVersions
            .Include(p => p.Submissions)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PortfolioVersion?> GetPortfolioVersionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PortfolioVersions
            .Include(p => p.Submissions)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PortfolioVersion> AddPortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default)
    {
        await _context.PortfolioVersions.AddAsync(portfolio, cancellationToken);
        return portfolio;
    }

    public Task UpdatePortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default)
    {
        _context.PortfolioVersions.Update(portfolio);
        return Task.CompletedTask;
    }

    public Task DeletePortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default)
    {
        _context.PortfolioVersions.Remove(portfolio);
        return Task.CompletedTask;
    }

    // Submissions
    public async Task<IReadOnlyList<ResumeSubmission>> GetAllSubmissionsAsync(Guid? profileId = null, Guid? versionId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ResumeSubmissions
            .Include(s => s.ResumeProfile)
            .Include(s => s.ResumeVersion)
            .Include(s => s.CoverLetter)
            .Include(s => s.PortfolioVersion)
            .AsNoTracking();

        if (profileId.HasValue)
        {
            query = query.Where(s => s.ResumeProfileId == profileId.Value);
        }

        if (versionId.HasValue)
        {
            query = query.Where(s => s.ResumeVersionId == versionId.Value);
        }

        return await query.OrderByDescending(s => s.SubmissionDate).ToListAsync(cancellationToken);
    }

    public async Task<ResumeSubmission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResumeSubmissions
            .Include(s => s.ResumeProfile)
            .Include(s => s.ResumeVersion)
            .Include(s => s.CoverLetter)
            .Include(s => s.PortfolioVersion)
            .Include(s => s.Snapshot)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ResumeSubmission> AddSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default)
    {
        await _context.ResumeSubmissions.AddAsync(submission, cancellationToken);
        return submission;
    }

    public Task UpdateSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default)
    {
        _context.ResumeSubmissions.Update(submission);
        return Task.CompletedTask;
    }

    public Task DeleteSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default)
    {
        _context.ResumeSubmissions.Remove(submission);
        return Task.CompletedTask;
    }

    // Snapshots
    public async Task<SubmissionSnapshot?> GetSnapshotBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return await _context.SubmissionSnapshots
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId, cancellationToken);
    }

    public async Task<SubmissionSnapshot> AddSnapshotAsync(SubmissionSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.SubmissionSnapshots.AddAsync(snapshot, cancellationToken);
        return snapshot;
    }
}

