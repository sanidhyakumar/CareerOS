using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ProfileCitizenshipConfiguration : IEntityTypeConfiguration<ProfileCitizenship>
{
    public void Configure(EntityTypeBuilder<ProfileCitizenship> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PassportNumber).HasMaxLength(100);
        builder.Property(c => c.Notes).HasMaxLength(1000);

        builder.HasOne(c => c.Country)
            .WithMany()
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.UserProfileId, c.CountryId });
    }
}

