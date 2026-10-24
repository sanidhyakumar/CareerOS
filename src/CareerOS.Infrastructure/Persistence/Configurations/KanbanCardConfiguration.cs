using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class KanbanCardConfiguration : IEntityTypeConfiguration<KanbanCard>
{
    public void Configure(EntityTypeBuilder<KanbanCard> builder)
    {
        builder.ToTable("KanbanCards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200);

        builder.Property(x => x.Salary)
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(150);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Application)
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.Column, x.Position });
    }
}

