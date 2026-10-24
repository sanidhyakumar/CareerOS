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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
