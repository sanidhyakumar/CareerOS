using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(10);
        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.IsActive);

        builder.HasMany(c => c.Regions)
            .WithOne(r => r.Country)
            .HasForeignKey(r => r.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

