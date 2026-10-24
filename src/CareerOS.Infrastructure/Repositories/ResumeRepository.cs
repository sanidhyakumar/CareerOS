using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly ApplicationDbContext _context;

    public ResumeRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Resume>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Resumes.OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Resume?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Resumes.FindAsync([id], cancellationToken);

    public async Task<Resume> AddAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        await _context.Resumes.AddAsync(resume, cancellationToken);
        return resume;
    }

    public Task UpdateAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        _context.Resumes.Update(resume);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resume = await GetByIdAsync(id, cancellationToken);
        if (resume != null) _context.Resumes.Remove(resume);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Resumes.CountAsync(cancellationToken);
}

