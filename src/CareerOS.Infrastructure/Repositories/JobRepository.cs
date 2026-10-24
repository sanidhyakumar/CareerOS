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

    public async Task<IEnumerable<Job>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Jobs.OrderByDescending(j => j.CreatedAt).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status, CancellationToken cancellationToken = default)
        => await _context.Jobs.Where(j => j.Status == status).ToListAsync(cancellationToken);

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Jobs.FindAsync([id], cancellationToken);

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

