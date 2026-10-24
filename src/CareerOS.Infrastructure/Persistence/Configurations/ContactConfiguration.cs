using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Company).HasMaxLength(200);
        builder.Property(c => c.JobTitle).HasMaxLength(200);
        builder.Property(c => c.Department).HasMaxLength(200);
        builder.Property(c => c.LinkedIn).HasMaxLength(500);
        builder.Property(c => c.Twitter).HasMaxLength(200);
        builder.Property(c => c.Location).HasMaxLength(200);
        builder.Property(c => c.Tags).HasMaxLength(500);
        builder.Property(c => c.Notes).HasMaxLength(4000);
        builder.Property(c => c.RelationshipNotes).HasMaxLength(4000);

        builder.HasOne(c => c.CompanyEntity)
            .WithMany(comp => comp.Contacts)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.JobEntity)
            .WithMany()
            .HasForeignKey(c => c.JobId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Interactions)
            .WithOne(i => i.Contact)
            .HasForeignKey(i => i.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Category);
        builder.HasIndex(c => c.CompanyId);
        builder.HasIndex(c => c.IsStarred);
        builder.HasIndex(c => c.NextFollowUpDate);
    }
}
