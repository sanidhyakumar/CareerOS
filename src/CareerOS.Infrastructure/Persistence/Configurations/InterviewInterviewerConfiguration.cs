using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class InterviewInterviewerConfiguration : IEntityTypeConfiguration<InterviewInterviewer>
{
    public void Configure(EntityTypeBuilder<InterviewInterviewer> builder)
    {
        builder.ToTable("InterviewInterviewers");

        builder.HasKey(ii => ii.Id);

        builder.Property(ii => ii.Role)
            .HasMaxLength(100);

        builder.HasOne(ii => ii.Interview)
            .WithMany(i => i.Interviewers)
            .HasForeignKey(ii => ii.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.Contact)
            .WithMany()
            .HasForeignKey(ii => ii.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ii => new { ii.InterviewId, ii.ContactId }).IsUnique();
    }
}
