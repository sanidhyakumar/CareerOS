using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ResumeSubmissionConfiguration : IEntityTypeConfiguration<ResumeSubmission>
{
    public void Configure(EntityTypeBuilder<ResumeSubmission> builder)
    {
        builder.ToTable("ResumeSubmissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.RoleTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Channel)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        builder.HasOne(s => s.ResumeProfile)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.ResumeProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.ResumeVersion)
            .WithMany(v => v.Submissions)
            .HasForeignKey(s => s.ResumeVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.CoverLetter)
            .WithMany(c => c.Submissions)
            .HasForeignKey(s => s.CoverLetterId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.PortfolioVersion)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.PortfolioVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Snapshot)
            .WithOne(snap => snap.Submission)
            .HasForeignKey<SubmissionSnapshot>(snap => snap.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.SubmissionDate);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Channel);
    }
}

