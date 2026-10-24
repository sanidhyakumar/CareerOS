using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand : IRequest<JobApplicationDto>
{
    public Guid Id { get; init; }
    public ApplicationStatus NewStatus { get; init; }
    public string? EventTitle { get; init; }
    public string? Notes { get; init; }
}
