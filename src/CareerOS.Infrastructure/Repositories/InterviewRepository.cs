using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Interview?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        return await _context.Interviews
            .Include(i => i.Company)
            .Include(i => i.Job)
            .Include(i => i.JobApplication)
            .Include(i => i.Interviewers)
                .ThenInclude(ii => ii.Contact)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Interview>> GetAllAsync(
        Guid? applicationId = null,
        Guid? companyId = null,
        Guid? jobId = null,
        InterviewType? type = null,
        InterviewStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Interviews
            .Include(i => i.Company)
            .Include(i => i.Job)
            .Include(i => i.JobApplication)
            .Include(i => i.Interviewers)
                .ThenInclude(ii => ii.Contact)
            .AsNoTracking();

        if (applicationId.HasValue)
        {
            query = query.Where(i => i.JobApplicationId == applicationId.Value);
        }

        if (companyId.HasValue)
        {
            query = query.Where(i => i.CompanyId == companyId.Value);
        }

        if (jobId.HasValue)
        {
            query = query.Where(i => i.JobId == jobId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(i => i.InterviewType == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(i => i.ScheduledAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(i => i.ScheduledAt <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(i =>
                i.Title.ToLower().Contains(term) ||
                (i.Company != null && i.Company.Name.ToLower().Contains(term)) ||
                (i.Job != null && i.Job.Title.ToLower().Contains(term)) ||
                (i.InterviewerNames != null && i.InterviewerNames.ToLower().Contains(term)) ||
                (i.QuestionsAsked != null && i.QuestionsAsked.ToLower().Contains(term)) ||
                (i.PreparationNotes != null && i.PreparationNotes.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(i => i.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Interview>> GetUpcomingAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Interviews
            .Include(i => i.Company)
            .Include(i => i.Job)
            .Include(i => i.Interviewers)
                .ThenInclude(ii => ii.Contact)
            .AsNoTracking()
            .Where(i => i.Status == InterviewStatus.Scheduled && i.ScheduledAt >= DateTime.UtcNow.AddHours(-1))
            .OrderBy(i => i.ScheduledAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Interview> AddAsync(Interview interview, CancellationToken cancellationToken = default)
    {
        await _context.Interviews.AddAsync(interview, cancellationToken);
        return interview;
    }

    public Task UpdateAsync(Interview interview, CancellationToken cancellationToken = default)
    {
        _context.Interviews.Update(interview);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Interview interview, CancellationToken cancellationToken = default)
    {
        _context.Interviews.Remove(interview);
        return Task.CompletedTask;
    }

    public async Task SetInterviewersAsync(Guid interviewId, IEnumerable<(Guid ContactId, string? Role)> interviewers, CancellationToken cancellationToken = default)
    {
        var existing = await _context.InterviewInterviewers
            .Where(ii => ii.InterviewId == interviewId)
            .ToListAsync(cancellationToken);

        _context.InterviewInterviewers.RemoveRange(existing);

        var newEntities = interviewers.Select(x => InterviewInterviewer.Create(interviewId, x.ContactId, x.Role));
        await _context.InterviewInterviewers.AddRangeAsync(newEntities, cancellationToken);
    }
}
