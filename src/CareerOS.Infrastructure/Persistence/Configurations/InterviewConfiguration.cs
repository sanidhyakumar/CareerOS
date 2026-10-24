using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerOS.Infrastructure.Persistence.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.MeetingLink)
            .HasMaxLength(1000);

        builder.Property(i => i.Location)
            .HasMaxLength(300);

        builder.Property(i => i.InterviewerNames)
            .HasMaxLength(500);

        builder.Property(i => i.PreparationNotes)
            .HasMaxLength(4000);

        builder.Property(i => i.QuestionsAsked)
            .HasMaxLength(4000);

        builder.Property(i => i.FeedbackNotes)
            .HasMaxLength(4000);

        builder.HasOne(i => i.JobApplication)
            .WithMany()
            .HasForeignKey(i => i.JobApplicationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Job)
            .WithMany()
            .HasForeignKey(i => i.JobId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Company)
            .WithMany()
            .HasForeignKey(i => i.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.Interviewers)
            .WithOne(ii => ii.Interview)
            .HasForeignKey(ii => ii.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.ScheduledAt);
        builder.HasIndex(i => i.InterviewType);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.CompanyId);
        builder.HasIndex(i => i.JobApplicationId);
    }
}

