using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Interview : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public Guid? JobApplicationId { get; set; }
    public JobApplication? JobApplication { get; set; }

    public Guid? JobId { get; set; }
    public Job? Job { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public InterviewType InterviewType { get; set; } = InterviewType.Technical;
    public InterviewFormat Format { get; set; } = InterviewFormat.VideoCall;
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public InterviewOutcome Outcome { get; set; } = InterviewOutcome.Pending;

    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public DateTime EndAt => ScheduledAt.AddMinutes(DurationMinutes);

    public string? MeetingLink { get; set; }
    public string? Location { get; set; }
    public string? InterviewerNames { get; set; }

    public string? PreparationNotes { get; set; }
    public string? QuestionsAsked { get; set; }
    public string? FeedbackNotes { get; set; }
    public int? Rating { get; set; } // 1 to 5 stars self-rating

    public ICollection<InterviewInterviewer> Interviewers { get; set; } = new List<InterviewInterviewer>();

    public Interview() { }

    public static Interview Create(
        string title,
        InterviewType interviewType,
        DateTime scheduledAt,
        int durationMinutes = 60,
        InterviewFormat format = InterviewFormat.VideoCall,
        Guid? jobApplicationId = null,
        Guid? jobId = null,
        Guid? companyId = null,
        string? meetingLink = null,
        string? location = null,
        string? interviewerNames = null,
        string? preparationNotes = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        return new Interview
        {
            Title = title.Trim(),
            InterviewType = interviewType,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes > 0 ? durationMinutes : 60,
            Format = format,
            JobApplicationId = jobApplicationId,
            JobId = jobId,
            CompanyId = companyId,
            MeetingLink = meetingLink?.Trim(),
            Location = location?.Trim(),
            InterviewerNames = interviewerNames?.Trim(),
            PreparationNotes = preparationNotes?.Trim(),
            Status = InterviewStatus.Scheduled,
            Outcome = InterviewOutcome.Pending
        };
    }

    public void Update(
        string title,
        InterviewType interviewType,
        DateTime scheduledAt,
        int durationMinutes,
        InterviewFormat format,
        InterviewStatus status,
        InterviewOutcome outcome,
        Guid? jobApplicationId,
        Guid? jobId,
        Guid? companyId,
        string? meetingLink,
        string? location,
        string? interviewerNames,
        string? preparationNotes,
        string? questionsAsked,
        string? feedbackNotes,
        int? rating)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        Title = title.Trim();
        InterviewType = interviewType;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes > 0 ? durationMinutes : 60;
        Format = format;
        Status = status;
        Outcome = outcome;
        JobApplicationId = jobApplicationId;
        JobId = jobId;
        CompanyId = companyId;
        MeetingLink = meetingLink?.Trim();
        Location = location?.Trim();
        InterviewerNames = interviewerNames?.Trim();
        PreparationNotes = preparationNotes?.Trim();
        QuestionsAsked = questionsAsked?.Trim();
        FeedbackNotes = feedbackNotes?.Trim();
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }
}
