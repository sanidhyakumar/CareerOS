using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class CredentialRepository : ICredentialRepository
{
    private readonly ApplicationDbContext _context;

    public CredentialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CredentialItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Credentials
            .Include(c => c.Company)
            .Include(c => c.JobApplication)
                .ThenInclude(ja => ja != null ? ja.Job : null)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CredentialItem>> GetAllAsync(Guid? companyId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Credentials
            .Include(c => c.Company)
            .Include(c => c.JobApplication)
                .ThenInclude(ja => ja != null ? ja.Job : null)
            .AsNoTracking();

        if (companyId.HasValue && companyId.Value != Guid.Empty)
        {
            query = query.Where(c => c.CompanyId == companyId.Value);
        }

        return await query.OrderByDescending(c => c.UpdatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CredentialItem item, CancellationToken cancellationToken = default)
    {
        await _context.Credentials.AddAsync(item, cancellationToken);
    }

    public Task UpdateAsync(CredentialItem item, CancellationToken cancellationToken = default)
    {
        _context.Credentials.Update(item);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Credentials.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.Credentials.Remove(item);
        }
    }
}
