namespace CareerOS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IJobRepository Jobs { get; }
    IContactRepository Contacts { get; }
    IResumeRepository Resumes { get; }
    ICountryRepository Countries { get; }
    IRegionRepository Regions { get; }
    IDocumentRepository Documents { get; }
    ITagRepository Tags { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
