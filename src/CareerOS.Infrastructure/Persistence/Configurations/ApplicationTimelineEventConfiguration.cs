using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class ApplicationTimelineEventConfiguration : IEntityTypeConfiguration<ApplicationTimelineEvent>
{
    public void Configure(EntityTypeBuilder<ApplicationTimelineEvent> builder)
    {
        builder.ToTable("ApplicationTimelineEvents");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(t => t.JobApplicationId);
        builder.HasIndex(t => t.EventDate);
    }
}

