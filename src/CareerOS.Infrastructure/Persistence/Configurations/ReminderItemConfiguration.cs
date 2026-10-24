using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ReminderItemConfiguration : IEntityTypeConfiguration<ReminderItem>
{
    public void Configure(EntityTypeBuilder<ReminderItem> builder)
    {
        builder.ToTable("ReminderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1500);

        builder.Property(x => x.EntityType)
            .HasMaxLength(50);

        builder.Property(x => x.ActionUrl)
            .HasMaxLength(250);

        builder.Property(x => x.DeduplicationKey)
            .HasMaxLength(100);

        builder.HasIndex(x => x.DeduplicationKey);
        builder.HasIndex(x => new { x.IsDismissed, x.IsSnoozed });
    }
}

