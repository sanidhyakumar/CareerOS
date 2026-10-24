using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.UpdateInterview;

public record UpdateInterviewCommand : IRequest<InterviewDto>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public InterviewType InterviewType { get; init; }
    public InterviewFormat Format { get; init; }
    public InterviewStatus Status { get; init; }
    public InterviewOutcome Outcome { get; init; }
    public DateTime ScheduledAt { get; init; }
    public int DurationMinutes { get; init; }
    public Guid? JobApplicationId { get; init; }
    public Guid? JobId { get; init; }
    public Guid? CompanyId { get; init; }
    public string? MeetingLink { get; init; }
    public string? Location { get; init; }
    public string? InterviewerNames { get; init; }
    public string? PreparationNotes { get; init; }
    public string? QuestionsAsked { get; init; }
    public string? FeedbackNotes { get; init; }
    public int? Rating { get; init; }
    public List<Guid>? InterviewerContactIds { get; init; }
}
