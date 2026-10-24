using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.UpdateApplication;

public record UpdateApplicationCommand : IRequest<JobApplicationDto>
{
    public Guid Id { get; init; }
    public Guid? ResumeVersionId { get; init; }
    public Guid? CoverLetterId { get; init; }
    public Guid? PortfolioVersionId { get; init; }
    public ApplicationStatus Status { get; init; }
    public DateTime ApplicationDate { get; init; }
    public string Channel { get; init; } = "Company Website";
    public Guid? ReferralContactId { get; init; }
    public string? Notes { get; init; }
}
