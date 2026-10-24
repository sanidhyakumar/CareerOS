using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class SponsorshipEvidenceConfiguration : IEntityTypeConfiguration<SponsorshipEvidence>
{
    public void Configure(EntityTypeBuilder<SponsorshipEvidence> builder)
    {
        builder.ToTable("SponsorshipEvidences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EvidenceType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
