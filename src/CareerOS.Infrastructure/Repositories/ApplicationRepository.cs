using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        return await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Company)
            .Include(a => a.ResumeVersion)
                .ThenInclude(v => v!.ResumeProfile)
            .Include(a => a.CoverLetter)
            .Include(a => a.PortfolioVersion)
            .Include(a => a.ReferralContact)
            .Include(a => a.Snapshot)
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<JobApplication>> GetAllAsync(
        Guid? jobId = null,
        Guid? companyId = null,
        ApplicationStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Company)
            .Include(a => a.ResumeVersion)
                .ThenInclude(v => v!.ResumeProfile)
            .Include(a => a.CoverLetter)
            .Include(a => a.PortfolioVersion)
            .Include(a => a.ReferralContact)
            .Include(a => a.Snapshot)
            .Include(a => a.TimelineEvents)
            .AsNoTracking();

        if (jobId.HasValue)
        {
            query = query.Where(a => a.JobId == jobId.Value);
        }

        if (companyId.HasValue)
        {
            query = query.Where(a => a.CompanyId == companyId.Value || (a.Job != null && a.Job.CompanyId == companyId.Value));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(a =>
                (a.Job != null && a.Job.Title.ToLower().Contains(term)) ||
                (a.Job != null && a.Job.Company.ToLower().Contains(term)) ||
                (a.Company != null && a.Company.Name.ToLower().Contains(term)) ||
                (a.Channel.ToLower().Contains(term)) ||
                (a.Notes != null && a.Notes.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(a => a.ApplicationDate)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default)
    {
        await _context.JobApplications.AddAsync(application, cancellationToken);
        return application;
    }

    public Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default)
    {
        _context.JobApplications.Update(application);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(JobApplication application, CancellationToken cancellationToken = default)
    {
        _context.JobApplications.Remove(application);
        return Task.CompletedTask;
    }

    public async Task<ApplicationTimelineEvent> AddTimelineEventAsync(ApplicationTimelineEvent timelineEvent, CancellationToken cancellationToken = default)
    {
        await _context.ApplicationTimelineEvents.AddAsync(timelineEvent, cancellationToken);
        return timelineEvent;
    }

    public async Task<IEnumerable<ApplicationTimelineEvent>> GetTimelineEventsAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationTimelineEvents
            .AsNoTracking()
            .Where(t => t.JobApplicationId == applicationId)
            .OrderBy(t => t.EventDate)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationSnapshot?> GetSnapshotAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.JobApplicationId == applicationId, cancellationToken);
    }

    public async Task<ApplicationSnapshot> AddSnapshotAsync(ApplicationSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.ApplicationSnapshots.AddAsync(snapshot, cancellationToken);
        return snapshot;
    }
}
