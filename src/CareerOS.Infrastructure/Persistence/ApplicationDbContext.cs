using CareerOS.Application.Common.Interfaces;
using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<DocumentTag> DocumentTags => Set<DocumentTag>();
    public DbSet<DocumentUsage> DocumentUsages => Set<DocumentUsage>();
    public DbSet<UserProfile> Profiles => Set<UserProfile>();
    public DbSet<ProfileCitizenship> ProfileCitizenships => Set<ProfileCitizenship>();
    public DbSet<WorkAuthorization> WorkAuthorizations => Set<WorkAuthorization>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<ResumeProfile> ResumeProfiles => Set<ResumeProfile>();
    public DbSet<ResumeVersion> ResumeVersions => Set<ResumeVersion>();
    public DbSet<CoverLetter> CoverLetters => Set<CoverLetter>();
    public DbSet<PortfolioVersion> PortfolioVersions => Set<PortfolioVersion>();
    public DbSet<ResumeSubmission> ResumeSubmissions => Set<ResumeSubmission>();
    public DbSet<SubmissionSnapshot> SubmissionSnapshots => Set<SubmissionSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
