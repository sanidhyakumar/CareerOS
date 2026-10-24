using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street1).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Street2).HasMaxLength(200);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.StateProvince).HasMaxLength(100);
        builder.Property(a => a.PostalCode).HasMaxLength(30);

        builder.HasOne(a => a.Country)
            .WithMany()
            .HasForeignKey(a => a.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Region)
            .WithMany()
            .HasForeignKey(a => a.RegionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.UserProfileId);
        builder.HasIndex(a => a.CountryId);
    }
}

