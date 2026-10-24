using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);
        builder.Property(r => r.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(r => new { r.CountryId, r.Code }).IsUnique();
    }
}

