using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class DocumentUsageConfiguration : IEntityTypeConfiguration<DocumentUsage>
{
    public void Configure(EntityTypeBuilder<DocumentUsage> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(u => u.EntityName).HasMaxLength(250);
        builder.Property(u => u.Purpose).HasMaxLength(500);

        builder.HasIndex(u => u.DocumentId);
        builder.HasIndex(u => new { u.EntityType, u.EntityId });
    }
}

