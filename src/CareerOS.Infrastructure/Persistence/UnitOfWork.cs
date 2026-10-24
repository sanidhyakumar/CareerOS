using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Repositories;

namespace CareerOS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IJobRepository? _jobs;
    private IContactRepository? _contacts;
    private IResumeRepository? _resumes;
    private ICountryRepository? _countries;
    private IRegionRepository? _regions;
    private IDocumentRepository? _documents;
    private ITagRepository? _tags;
    private IUserProfileRepository? _profiles;
    private IResumeCenterRepository? _resumeCenter;
    private ICompanyRepository? _companies;
    private IApplicationRepository? _applications;
    private IInterviewRepository? _interviews;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IJobRepository Jobs => _jobs ??= new JobRepository(_context);
    public IContactRepository Contacts => _contacts ??= new ContactRepository(_context);
    public IResumeRepository Resumes => _resumes ??= new ResumeRepository(_context);
    public ICountryRepository Countries => _countries ??= new CountryRepository(_context);
    public IRegionRepository Regions => _regions ??= new RegionRepository(_context);
    public IDocumentRepository Documents => _documents ??= new DocumentRepository(_context);
    public ITagRepository Tags => _tags ??= new TagRepository(_context);
    public IUserProfileRepository Profiles => _profiles ??= new UserProfileRepository(_context);
    public IResumeCenterRepository ResumeCenter => _resumeCenter ??= new ResumeCenterRepository(_context);
    public ICompanyRepository Companies => _companies ??= new CompanyRepository(_context);
    public IApplicationRepository Applications => _applications ??= new ApplicationRepository(_context);
    public IInterviewRepository Interviews => _interviews ??= new InterviewRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
