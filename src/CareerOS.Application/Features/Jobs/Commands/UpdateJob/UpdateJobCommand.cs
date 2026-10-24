using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.UpdateJob;

public record UpdateJobCommand(
    Guid Id,
    string Title,
    string Company,
    string? Location,
    string? Url,
    string? Description,
    string? Notes,
    JobStatus Status,
    string? Salary,
    DateTime? AppliedAt,
    DateTime? InterviewAt
) : IRequest<JobDto>;

