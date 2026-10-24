using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ResumeVersionConfiguration : IEntityTypeConfiguration<ResumeVersion>
{
    public void Configure(EntityTypeBuilder<ResumeVersion> builder)
    {
        builder.ToTable("ResumeVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VersionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.FileFormat)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.TargetCompany)
            .HasMaxLength(200);

        builder.Property(v => v.TargetJobTitle)
            .HasMaxLength(200);

        builder.Property(v => v.TailoredKeywords)
            .HasMaxLength(1000);

        builder.Property(v => v.ChangeNotes)
            .HasMaxLength(1000);

        builder.HasOne(v => v.ResumeProfile)
            .WithMany(p => p.Versions)
            .HasForeignKey(v => v.ResumeProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Document)
            .WithMany()
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(v => new { v.ResumeProfileId, v.VersionNumber });
        builder.HasIndex(v => v.IsBaseVersion);
    }
}

