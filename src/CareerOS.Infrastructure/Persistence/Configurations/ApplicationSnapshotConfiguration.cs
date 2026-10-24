using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ApplicationSnapshotConfiguration : IEntityTypeConfiguration<ApplicationSnapshot>
{
    public void Configure(EntityTypeBuilder<ApplicationSnapshot> builder)
    {
        builder.ToTable("ApplicationSnapshots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.JobTitle)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Channel)
            .HasMaxLength(100);

        builder.Property(s => s.SubmissionNotes)
            .HasMaxLength(4000);

        builder.HasIndex(s => s.JobApplicationId).IsUnique();
        builder.HasIndex(s => s.SnapshotDate);
    }
}
