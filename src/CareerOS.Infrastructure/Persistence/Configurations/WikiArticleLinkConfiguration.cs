using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class WikiArticleLinkConfiguration : IEntityTypeConfiguration<WikiArticleLink>
{
    public void Configure(EntityTypeBuilder<WikiArticleLink> builder)
    {
        builder.ToTable("WikiArticleLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TargetSlug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.LinkText)
            .HasMaxLength(250);

        builder.HasIndex(x => new { x.SourceArticleId, x.TargetSlug });
    }
}

