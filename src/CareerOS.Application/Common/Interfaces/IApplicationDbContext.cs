using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Job> Jobs { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Resume> Resumes { get; }
    DbSet<Country> Countries { get; }
    DbSet<Region> Regions { get; }
    DbSet<Document> Documents { get; }
    DbSet<Tag> Tags { get; }
    DbSet<DocumentTag> DocumentTags { get; }
    DbSet<DocumentUsage> DocumentUsages { get; }
    DbSet<UserProfile> Profiles { get; }
    DbSet<ProfileCitizenship> ProfileCitizenships { get; }
    DbSet<WorkAuthorization> WorkAuthorizations { get; }
    DbSet<SocialLink> SocialLinks { get; }
    DbSet<Address> Addresses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
