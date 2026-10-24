using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class PortfolioVersionConfiguration : IEntityTypeConfiguration<PortfolioVersion>
{
    public void Configure(EntityTypeBuilder<PortfolioVersion> builder)
    {
        builder.ToTable("PortfolioVersions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Url)
            .HasMaxLength(500);

        builder.Property(p => p.TargetRole)
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.FeaturedProjects)
            .HasMaxLength(4000);

        builder.HasIndex(p => p.IsDefault);
    }
}

