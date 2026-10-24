using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.MiddleName).HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PreferredName).HasMaxLength(100);
        builder.Property(p => p.Headline).HasMaxLength(250);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(255);
        builder.Property(p => p.SecondaryEmail).HasMaxLength(255);
        builder.Property(p => p.PhoneNumber).HasMaxLength(50);
        builder.Property(p => p.SecondaryPhone).HasMaxLength(50);
        builder.Property(p => p.Bio).HasMaxLength(4000);
        builder.Property(p => p.AvatarUrl).HasMaxLength(1000);

        builder.HasMany(p => p.Citizenships)
            .WithOne(c => c.UserProfile)
            .HasForeignKey(c => c.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.WorkAuthorizations)
            .WithOne(w => w.UserProfile)
            .HasForeignKey(w => w.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.SocialLinks)
            .WithOne(s => s.UserProfile)
            .HasForeignKey(s => s.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Addresses)
            .WithOne(a => a.UserProfile)
            .HasForeignKey(a => a.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

