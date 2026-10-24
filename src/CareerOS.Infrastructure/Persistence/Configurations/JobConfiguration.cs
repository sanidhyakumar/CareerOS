using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Title).IsRequired().HasMaxLength(200);
        builder.Property(j => j.Company).IsRequired().HasMaxLength(200);
        builder.Property(j => j.Location).HasMaxLength(200);
        builder.Property(j => j.Url).HasMaxLength(500);
        builder.Property(j => j.Salary).HasMaxLength(100);
        builder.Property(j => j.Status).HasConversion<int>();
        builder.Property(j => j.Priority).HasConversion<int>();
        builder.Property(j => j.EmploymentType).HasConversion<int>();
        builder.Property(j => j.WorkplaceType).HasConversion<int>();
        builder.Property(j => j.Seniority).HasConversion<int>();
        builder.Property(j => j.Department).HasMaxLength(150);
        builder.Property(j => j.TechStack).HasMaxLength(500);
        builder.Property(j => j.Equity).HasMaxLength(200);
        builder.Property(j => j.Currency).HasMaxLength(10);
        builder.Property(j => j.MinSalary).HasPrecision(18, 2);
        builder.Property(j => j.MaxSalary).HasPrecision(18, 2);

        builder.HasOne(j => j.CompanyEntity)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(j => j.CompanyId);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.Priority);
        builder.HasIndex(j => j.EmploymentType);
        builder.HasIndex(j => j.CreatedAt);
    }
}

