using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class CareerProfileConfiguration : IEntityTypeConfiguration<CareerProfile>
{
    public void Configure(EntityTypeBuilder<CareerProfile> builder)
    {
        builder.ToTable("CareerProfiles");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cp => cp.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(cp => cp.Slug)
            .IsUnique();

        builder.Property(cp => cp.Relationship)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cp => cp.Email)
            .HasMaxLength(200);

        builder.Property(cp => cp.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(cp => cp.ColorCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(cp => cp.Headline)
            .HasMaxLength(300);

        builder.Property(cp => cp.Bio)
            .HasMaxLength(2000);

        builder.HasOne(cp => cp.Preferences)
            .WithOne(p => p.CareerProfile)
            .HasForeignKey<CareerProfilePreference>(p => p.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.Addresses)
            .WithOne(a => a.CareerProfile)
            .HasForeignKey(a => a.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.Documents)
            .WithOne(d => d.CareerProfile)
            .HasForeignKey(d => d.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.JobApplications)
            .WithOne(a => a.CareerProfile)
            .HasForeignKey(a => a.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.Contacts)
            .WithOne(c => c.CareerProfile)
            .HasForeignKey(c => c.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.Interviews)
            .WithOne(i => i.CareerProfile)
            .HasForeignKey(i => i.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.SponsorshipCases)
            .WithOne(s => s.CareerProfile)
            .HasForeignKey(s => s.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.WikiArticles)
            .WithOne(w => w.CareerProfile)
            .HasForeignKey(w => w.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.ResumeProfiles)
            .WithOne(r => r.CareerProfile)
            .HasForeignKey(r => r.CareerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CareerProfilePreferenceConfiguration : IEntityTypeConfiguration<CareerProfilePreference>
{
    public void Configure(EntityTypeBuilder<CareerProfilePreference> builder)
    {
        builder.ToTable("CareerProfilePreferences");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TargetRoles)
            .HasMaxLength(500);

        builder.Property(p => p.TargetLocations)
            .HasMaxLength(500);

        builder.Property(p => p.Currency)
            .HasMaxLength(10);

        builder.Property(p => p.RemotePreference)
            .HasMaxLength(50);
    }
}

public class CareerProfileAddressConfiguration : IEntityTypeConfiguration<CareerProfileAddress>
{
    public void Configure(EntityTypeBuilder<CareerProfileAddress> builder)
    {
        builder.ToTable("CareerProfileAddresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type)
            .HasMaxLength(50);

        builder.Property(a => a.Street)
            .HasMaxLength(250);

        builder.Property(a => a.City)
            .HasMaxLength(100);

        builder.Property(a => a.StateOrProvince)
            .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
            .HasMaxLength(50);

        builder.Property(a => a.Country)
            .HasMaxLength(100);
    }
}

