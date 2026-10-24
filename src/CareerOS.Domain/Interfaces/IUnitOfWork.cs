namespace CareerOS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IJobRepository Jobs { get; }
    IContactRepository Contacts { get; }
    IResumeRepository Resumes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

