using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class SponsorshipCaseConfiguration : IEntityTypeConfiguration<SponsorshipCase>
{
    public void Configure(EntityTypeBuilder<SponsorshipCase> builder)
    {
        builder.ToTable("SponsorshipCases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProgramName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Currency)
            .HasMaxLength(10);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.EvidenceList)
            .WithOne(x => x.SponsorshipCase)
            .HasForeignKey(x => x.SponsorshipCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

