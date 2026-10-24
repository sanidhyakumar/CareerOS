using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class KanbanRepository : IKanbanRepository
{
    private readonly ApplicationDbContext _context;

    public KanbanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KanbanCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KanbanCards
            .Include(c => c.Company)
            .Include(c => c.Job)
            .Include(c => c.Application)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<KanbanCard>> GetAllAsync(KanbanColumnType? column = null, CancellationToken cancellationToken = default)
    {
        var query = _context.KanbanCards
            .Include(c => c.Company)
            .Include(c => c.Job)
            .Include(c => c.Application)
            .AsQueryable();

        if (column.HasValue)
        {
            query = query.Where(c => c.Column == column.Value);
        }

        return await query
            .OrderBy(c => c.Position)
            .ThenByDescending(c => c.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<KanbanCard> AddAsync(KanbanCard card, CancellationToken cancellationToken = default)
    {
        await _context.KanbanCards.AddAsync(card, cancellationToken);
        return card;
    }

    public Task UpdateAsync(KanbanCard card, CancellationToken cancellationToken = default)
    {
        _context.KanbanCards.Update(card);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(KanbanCard card, CancellationToken cancellationToken = default)
    {
        _context.KanbanCards.Remove(card);
        return Task.CompletedTask;
    }

    public async Task ReorderCardsAsync(KanbanColumnType column, List<Guid> cardIdsInOrder, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < cardIdsInOrder.Count; i++)
        {
            var id = cardIdsInOrder[i];
            var card = await _context.KanbanCards.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (card != null)
            {
                card.Column = column;
                card.Position = i;
                card.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public async Task SyncWithJobsAndApplicationsAsync(CancellationToken cancellationToken = default)
    {
        // 1. Sync Job Applications
        var apps = await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Company)
            .ToListAsync(cancellationToken);

        var existingCardAppIds = await _context.KanbanCards
            .Where(c => c.ApplicationId != null)
            .Select(c => c.ApplicationId!.Value)
            .ToListAsync(cancellationToken);

        foreach (var app in apps)
        {
            if (!existingCardAppIds.Contains(app.Id))
            {
                var targetColumn = app.Status switch
                {
                    ApplicationStatus.Draft => KanbanColumnType.Saved,
                    ApplicationStatus.Applied => KanbanColumnType.Applied,
                    ApplicationStatus.UnderReview => KanbanColumnType.Applied,
                    ApplicationStatus.PhoneScreen => KanbanColumnType.Interviewing,
                    ApplicationStatus.TechnicalInterview => KanbanColumnType.Interviewing,
                    ApplicationStatus.BehavioralInterview => KanbanColumnType.Interviewing,
                    ApplicationStatus.FinalRound => KanbanColumnType.Interviewing,
                    ApplicationStatus.OfferReceived => KanbanColumnType.Offer,
                    ApplicationStatus.Accepted => KanbanColumnType.Offer,
                    ApplicationStatus.Rejected => KanbanColumnType.Rejected,
                    ApplicationStatus.Withdrawn => KanbanColumnType.Rejected,
                    _ => KanbanColumnType.Applied
                };

                var maxPos = await _context.KanbanCards
                    .Where(c => c.Column == targetColumn)
                    .Select(c => (int?)c.Position)
                    .MaxAsync(cancellationToken) ?? 0;

                var card = KanbanCard.Create(
                    title: app.Job?.Title ?? "Application Opportunity",
                    companyName: app.Company?.Name ?? "Company",
                    column: targetColumn,
                    position: maxPos + 1,
                    companyId: app.CompanyId,
                    jobId: app.JobId,
                    applicationId: app.Id,
                    priority: app.Job?.Priority ?? JobPriority.Medium,
                    salary: app.Job?.Salary,
                    location: app.Job?.Location,
                    notes: app.Notes,
                    tags: app.Job?.TechStack);

                await _context.KanbanCards.AddAsync(card, cancellationToken);
            }
        }

        // 2. Sync standalone Jobs that don't have an application or card yet
        var jobs = await _context.Jobs
            .Include(j => j.CompanyEntity)
            .ToListAsync(cancellationToken);

        var existingCardJobIds = await _context.KanbanCards
            .Where(c => c.JobId != null)
            .Select(c => c.JobId!.Value)
            .ToListAsync(cancellationToken);

        foreach (var job in jobs)
        {
            if (!existingCardJobIds.Contains(job.Id))
            {
                var targetColumn = job.Status switch
                {
                    JobStatus.Saved => KanbanColumnType.Saved,
                    JobStatus.Applied => KanbanColumnType.Applied,
                    JobStatus.PhoneScreen => KanbanColumnType.Interviewing,
                    JobStatus.Interview => KanbanColumnType.Interviewing,
                    JobStatus.Offer => KanbanColumnType.Offer,
                    JobStatus.Accepted => KanbanColumnType.Offer,
                    JobStatus.Rejected => KanbanColumnType.Rejected,
                    JobStatus.Withdrawn => KanbanColumnType.Rejected,
                    _ => KanbanColumnType.Saved
                };

                var maxPos = await _context.KanbanCards
                    .Where(c => c.Column == targetColumn)
                    .Select(c => (int?)c.Position)
                    .MaxAsync(cancellationToken) ?? 0;

                var card = KanbanCard.Create(
                    title: job.Title,
                    companyName: job.CompanyEntity != null ? job.CompanyEntity.Name : job.Company,
                    column: targetColumn,
                    position: maxPos + 1,
                    companyId: job.CompanyId,
                    jobId: job.Id,
                    applicationId: null,
                    priority: job.Priority,
                    salary: job.Salary,
                    location: job.Location,
                    notes: job.Notes,
                    tags: job.TechStack);

                await _context.KanbanCards.AddAsync(card, cancellationToken);
            }
        }
    }
}
