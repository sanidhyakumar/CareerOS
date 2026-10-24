using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ContactInteractionConfiguration : IEntityTypeConfiguration<ContactInteraction>
{
    public void Configure(EntityTypeBuilder<ContactInteraction> builder)
    {
        builder.ToTable("ContactInteractions");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Summary).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Channel).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Notes).HasMaxLength(4000);

        builder.HasIndex(i => i.ContactId);
        builder.HasIndex(i => i.InteractionDate);
        builder.HasIndex(i => i.FollowUpDate);
    }
}
