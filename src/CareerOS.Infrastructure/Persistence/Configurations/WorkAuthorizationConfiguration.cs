using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class WorkAuthorizationConfiguration : IEntityTypeConfiguration<WorkAuthorization>
{
    public void Configure(EntityTypeBuilder<WorkAuthorization> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.VisaType).HasMaxLength(100);
        builder.Property(w => w.Notes).HasMaxLength(1000);

        builder.HasOne(w => w.Country)
            .WithMany()
            .HasForeignKey(w => w.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Document)
            .WithMany()
            .HasForeignKey(w => w.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(w => w.UserProfileId);
        builder.HasIndex(w => w.CountryId);
        builder.HasIndex(w => w.AuthorizationType);
    }
}

