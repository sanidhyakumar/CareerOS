using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Repositories;

namespace CareerOS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IJobRepository? _jobs;
    private IContactRepository? _contacts;
    private IResumeRepository? _resumes;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IJobRepository Jobs => _jobs ??= new JobRepository(_context);
    public IContactRepository Contacts => _contacts ??= new ContactRepository(_context);
    public IResumeRepository Resumes => _resumes ??= new ResumeRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

