using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class ApplicationTimelineEvent : BaseEntity
{
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public ApplicationStatus Status { get; set; }
    public DateTime EventDate { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public ApplicationTimelineEvent() { }

    public static ApplicationTimelineEvent Create(
        Guid jobApplicationId,
        ApplicationStatus status,
        DateTime eventDate,
        string title,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        return new ApplicationTimelineEvent
        {
            JobApplicationId = jobApplicationId,
            Status = status,
            EventDate = eventDate,
            Title = title.Trim(),
            Notes = notes?.Trim()
        };
    }
}

