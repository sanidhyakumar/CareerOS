using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class CoverLetterConfiguration : IEntityTypeConfiguration<CoverLetter>
{
    public void Configure(EntityTypeBuilder<CoverLetter> builder)
    {
        builder.ToTable("CoverLetters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.TargetCompany)
            .HasMaxLength(200);

        builder.Property(c => c.TargetRole)
            .HasMaxLength(200);

        builder.Property(c => c.RecipientName)
            .HasMaxLength(200);

        builder.Property(c => c.RecipientTitle)
            .HasMaxLength(200);

        builder.Property(c => c.Body)
            .IsRequired();

        builder.HasOne(c => c.ResumeProfile)
            .WithMany(p => p.CoverLetters)
            .HasForeignKey(c => c.ResumeProfileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.IsTemplate);
    }
}

