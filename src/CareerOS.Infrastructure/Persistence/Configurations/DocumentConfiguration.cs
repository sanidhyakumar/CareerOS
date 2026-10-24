using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.OriginalFileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.FileExtension).IsRequired().HasMaxLength(20);
        builder.Property(d => d.ContentType).IsRequired().HasMaxLength(150);
        builder.Property(d => d.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(d => d.Sha256Hash).IsRequired().HasMaxLength(64);
        builder.Property(d => d.Description).HasMaxLength(1000);
        builder.Property(d => d.DocumentNumber).HasMaxLength(100);
        builder.Property(d => d.IssuingAuthority).HasMaxLength(200);

        builder.HasIndex(d => d.Sha256Hash);
        builder.HasIndex(d => d.IsLatestVersion);
        builder.HasIndex(d => d.RootDocumentId);
        builder.HasIndex(d => d.CreatedAt);
        builder.HasIndex(d => d.Category);
        builder.HasIndex(d => d.ExpiryDate);

        builder.HasMany(d => d.Usages)
            .WithOne(u => u.Document)
            .HasForeignKey(u => u.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.RootDocument)
            .WithMany(d => d.ChildVersions)
            .HasForeignKey(d => d.RootDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Country)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.CountryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Region)
            .WithMany(r => r.Documents)
            .HasForeignKey(d => d.RegionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

