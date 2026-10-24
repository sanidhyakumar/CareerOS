using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Channel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Notes)
            .HasMaxLength(4000);

        builder.HasOne(a => a.Job)
            .WithMany()
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Company)
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.ResumeVersion)
            .WithMany()
            .HasForeignKey(a => a.ResumeVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.CoverLetter)
            .WithMany()
            .HasForeignKey(a => a.CoverLetterId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.PortfolioVersion)
            .WithMany()
            .HasForeignKey(a => a.PortfolioVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.ReferralContact)
            .WithMany()
            .HasForeignKey(a => a.ReferralContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Snapshot)
            .WithOne(s => s.JobApplication)
            .HasForeignKey<ApplicationSnapshot>(s => s.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.TimelineEvents)
            .WithOne(t => t.JobApplication)
            .HasForeignKey(t => t.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.JobId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.ApplicationDate);
    }
}

