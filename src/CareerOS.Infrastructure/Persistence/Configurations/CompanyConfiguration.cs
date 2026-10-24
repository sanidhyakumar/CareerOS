using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Headquarters)
            .HasMaxLength(200);

        builder.Property(c => c.Size)
            .HasMaxLength(100);

        builder.Property(c => c.Stage)
            .HasMaxLength(100);

        builder.Property(c => c.LogoUrl)
            .HasMaxLength(500);

        builder.Property(c => c.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(c => c.CareersUrl)
            .HasMaxLength(500);

        builder.Property(c => c.TechBlogUrl)
            .HasMaxLength(500);

        builder.Property(c => c.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(c => c.GlassdoorUrl)
            .HasMaxLength(500);

        builder.Property(c => c.GlassdoorRating)
            .HasPrecision(3, 1);

        builder.Property(c => c.Industry)
            .HasMaxLength(150);

        builder.Property(c => c.Sector)
            .HasMaxLength(150);

        builder.Property(c => c.Tags)
            .HasMaxLength(500);

        builder.Property(c => c.SponsoredVisas)
            .HasMaxLength(200);

        builder.Property(c => c.SponsorshipNotes)
            .HasMaxLength(1000);

        builder.Property(c => c.RelocationNotes)
            .HasMaxLength(1000);

        builder.Property(c => c.Notes)
            .HasMaxLength(4000);

        builder.Property(c => c.Pros)
            .HasMaxLength(2000);

        builder.Property(c => c.Cons)
            .HasMaxLength(2000);

        builder.HasMany(c => c.Jobs)
            .WithOne(j => j.CompanyEntity)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Contacts)
            .WithOne(ct => ct.CompanyEntity)
            .HasForeignKey(ct => ct.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.Industry);
        builder.HasIndex(c => c.SponsorshipStatus);
        builder.HasIndex(c => c.RelocationStatus);
        builder.HasIndex(c => c.IsTarget);
    }
}

