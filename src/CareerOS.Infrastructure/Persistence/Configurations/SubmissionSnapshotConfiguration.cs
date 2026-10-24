using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class SubmissionSnapshotConfiguration : IEntityTypeConfiguration<SubmissionSnapshot>
{
    public void Configure(EntityTypeBuilder<SubmissionSnapshot> builder)
    {
        builder.ToTable("SubmissionSnapshots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ResumeProfileName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.ResumeVersionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.ResumeDocumentFileName)
            .HasMaxLength(300);

        builder.Property(s => s.ResumeDocumentSha256)
            .HasMaxLength(64);

        builder.Property(s => s.CoverLetterTitle)
            .HasMaxLength(200);

        builder.Property(s => s.PortfolioName)
            .HasMaxLength(200);

        builder.Property(s => s.PortfolioUrlSnapshot)
            .HasMaxLength(500);

        builder.HasOne(s => s.Submission)
            .WithOne(sub => sub.Snapshot)
            .HasForeignKey<SubmissionSnapshot>(s => s.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.SubmissionId).IsUnique();
        builder.HasIndex(s => s.SnapshotDate);
    }
}

