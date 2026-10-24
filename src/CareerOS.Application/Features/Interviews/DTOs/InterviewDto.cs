using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Interviews.DTOs;

public class InterviewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public InterviewType InterviewType { get; set; }
    public string TypeName => InterviewType.ToString();

    public InterviewFormat Format { get; set; }
    public string FormatName => Format.ToString();

    public InterviewStatus Status { get; set; }
    public string StatusName => Status.ToString();

    public InterviewOutcome Outcome { get; set; }
    public string OutcomeName => Outcome.ToString();

    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime EndAt => ScheduledAt.AddMinutes(DurationMinutes);

    public string? MeetingLink { get; set; }
    public string? Location { get; set; }
    public string? InterviewerNames { get; set; }

    public Guid? JobApplicationId { get; set; }
    public Guid? JobId { get; set; }
    public string? JobTitle { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string? PreparationNotes { get; set; }
    public string? QuestionsAsked { get; set; }
    public string? FeedbackNotes { get; set; }
    public int? Rating { get; set; }

    public List<InterviewInterviewerDto> Interviewers { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InterviewInterviewerDto
{
    public Guid ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactLinkedIn { get; set; }
}

public class InterviewStatsDto
{
    public int TotalInterviews { get; set; }
    public int UpcomingCount { get; set; }
    public int CompletedCount { get; set; }
    public int TechnicalCount { get; set; }
    public int SystemDesignCount { get; set; }
    public int BehavioralCount { get; set; }
    public int PassedCount { get; set; }
    public decimal PassRate { get; set; }
}
