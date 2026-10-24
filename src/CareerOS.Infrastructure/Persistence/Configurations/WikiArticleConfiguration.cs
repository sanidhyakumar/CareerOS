using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class WikiArticleConfiguration : IEntityTypeConfiguration<WikiArticle>
{
    public void Configure(EntityTypeBuilder<WikiArticle> builder)
    {
        builder.ToTable("WikiArticles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Summary)
            .HasMaxLength(1000);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Outlinks)
            .WithOne(x => x.SourceArticle)
            .HasForeignKey(x => x.SourceArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Inlinks)
            .WithOne(x => x.TargetArticle)
            .HasForeignKey(x => x.TargetArticleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
