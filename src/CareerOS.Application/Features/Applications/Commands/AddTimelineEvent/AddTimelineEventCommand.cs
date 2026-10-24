using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.AddTimelineEvent;

public record AddTimelineEventCommand : IRequest<ApplicationTimelineEventDto>
{
    public Guid ApplicationId { get; init; }
    public ApplicationStatus Status { get; init; }
    public DateTime EventDate { get; init; } = DateTime.UtcNow;
    public string Title { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
