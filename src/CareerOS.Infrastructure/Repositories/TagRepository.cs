using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tags.OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Tags.FindAsync([id], cancellationToken);

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return await _context.Tags.FirstOrDefaultAsync(t => t.Name == normalized, cancellationToken);
    }

    public async Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await _context.Tags.AddAsync(tag, cancellationToken);
        return tag;
    }

    public Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _context.Tags.Update(tag);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FindAsync([id], cancellationToken);
        if (tag != null) _context.Tags.Remove(tag);
    }

    public async Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        return await _context.Tags.Where(t => idSet.Contains(t.Id)).ToListAsync(cancellationToken);
    }
}

