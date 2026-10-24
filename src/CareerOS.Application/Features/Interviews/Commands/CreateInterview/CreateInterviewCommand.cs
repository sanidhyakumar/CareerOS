using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.CreateInterview;

public record CreateInterviewCommand : IRequest<InterviewDto>
{
    public string Title { get; init; } = string.Empty;
    public InterviewType InterviewType { get; init; } = InterviewType.Technical;
    public InterviewFormat Format { get; init; } = InterviewFormat.VideoCall;
    public DateTime ScheduledAt { get; init; }
    public int DurationMinutes { get; init; } = 60;
    public Guid? JobApplicationId { get; init; }
    public Guid? JobId { get; init; }
    public Guid? CompanyId { get; init; }
    public string? MeetingLink { get; init; }
    public string? Location { get; init; }
    public string? InterviewerNames { get; init; }
    public string? PreparationNotes { get; init; }
    public List<Guid>? InterviewerContactIds { get; init; }
}

