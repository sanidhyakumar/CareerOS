using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.CreateApplication;

public record CreateApplicationCommand : IRequest<JobApplicationDto>
{
    public Guid JobId { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? ResumeVersionId { get; init; }
    public Guid? CoverLetterId { get; init; }
    public Guid? PortfolioVersionId { get; init; }
    public ApplicationStatus Status { get; init; } = ApplicationStatus.Applied;
    public DateTime ApplicationDate { get; init; } = DateTime.UtcNow;
    public string Channel { get; init; } = "Company Website";
    public Guid? ReferralContactId { get; init; }
    public string? Notes { get; init; }
}

