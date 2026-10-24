using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class SecuritySettingConfiguration : IEntityTypeConfiguration<SecuritySetting>
{
    public void Configure(EntityTypeBuilder<SecuritySetting> builder)
    {
        builder.HasKey(s => s.Key);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired();
    }
}
