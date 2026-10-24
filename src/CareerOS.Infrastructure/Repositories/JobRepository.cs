using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Job>> GetAllAsync(
        string? searchTerm = null,
        Guid? companyId = null,
        JobStatus? status = null,
        JobPriority? priority = null,
        EmploymentType? employmentType = null,
        WorkplaceType? workplaceType = null,
        SeniorityLevel? seniority = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs
            .Include(j => j.CompanyEntity)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(term) ||
                j.Company.ToLower().Contains(term) ||
                (j.Description != null && j.Description.ToLower().Contains(term)) ||
                (j.TechStack != null && j.TechStack.ToLower().Contains(term)) ||
                (j.Department != null && j.Department.ToLower().Contains(term)));
        }

        if (companyId.HasValue)
        {
            query = query.Where(j => j.CompanyId == companyId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(j => j.Priority == priority.Value);
        }

        if (employmentType.HasValue)
        {
            query = query.Where(j => j.EmploymentType == employmentType.Value);
        }

        if (workplaceType.HasValue)
        {
            query = query.Where(j => j.WorkplaceType == workplaceType.Value);
        }

        if (seniority.HasValue)
        {
            query = query.Where(j => j.Seniority == seniority.Value);
        }

        return await query
            .OrderByDescending(j => j.Priority)
            .ThenByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status, CancellationToken cancellationToken = default)
        => await _context.Jobs
            .Include(j => j.CompanyEntity)
            .Where(j => j.Status == status)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        => await _context.Jobs
            .Include(j => j.CompanyEntity)
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Jobs
            .Include(j => j.CompanyEntity)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

    public async Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        await _context.Jobs.AddAsync(job, cancellationToken);
        return job;
    }

    public Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        _context.Jobs.Update(job);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await GetByIdAsync(id, cancellationToken);
        if (job != null) _context.Jobs.Remove(job);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Jobs.CountAsync(cancellationToken);
}
