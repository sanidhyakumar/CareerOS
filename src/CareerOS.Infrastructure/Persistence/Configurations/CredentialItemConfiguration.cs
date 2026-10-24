using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class CredentialItemConfiguration : IEntityTypeConfiguration<CredentialItem>
{
    public void Configure(EntityTypeBuilder<CredentialItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ServiceName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Username)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.EncryptedPassword)
            .IsRequired();

        builder.Property(c => c.PortalUrl)
            .HasMaxLength(500);

        builder.HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.JobApplication)
            .WithMany()
            .HasForeignKey(c => c.JobApplicationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.ServiceName);
        builder.HasIndex(c => c.CompanyId);
    }
}

