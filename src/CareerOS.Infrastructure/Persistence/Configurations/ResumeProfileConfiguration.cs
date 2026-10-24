using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ResumeProfileConfiguration : IEntityTypeConfiguration<ResumeProfile>
{
    public void Configure(EntityTypeBuilder<ResumeProfile> builder)
    {
        builder.ToTable("ResumeProfiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.TargetRole)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.TargetIndustry)
            .HasMaxLength(200);

        builder.Property(p => p.Summary)
            .HasMaxLength(2000);

        builder.Property(p => p.PrimarySkills)
            .HasMaxLength(1000);

        builder.Property(p => p.SecondarySkills)
            .HasMaxLength(1000);

        builder.HasMany(p => p.Versions)
            .WithOne(v => v.ResumeProfile)
            .HasForeignKey(v => v.ResumeProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CoverLetters)
            .WithOne(c => c.ResumeProfile)
            .HasForeignKey(c => c.ResumeProfileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Submissions)
            .WithOne(s => s.ResumeProfile)
            .HasForeignKey(s => s.ResumeProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.IsDefault);
        builder.HasIndex(p => p.IsArchived);
    }
}

