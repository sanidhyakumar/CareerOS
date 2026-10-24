using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class SocialLinkConfiguration : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Url).IsRequired().HasMaxLength(500);
        builder.Property(s => s.DisplayLabel).HasMaxLength(100);

        builder.HasIndex(s => s.UserProfileId);
        builder.HasIndex(s => new { s.UserProfileId, s.Platform });
    }
}

