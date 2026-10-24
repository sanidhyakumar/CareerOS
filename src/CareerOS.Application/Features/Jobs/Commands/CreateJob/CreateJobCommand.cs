using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.CreateJob;

public record CreateJobCommand(
    string Title,
    string Company,
    string? Location,
    string? Url,
    string? Description,
    string? Notes,
    JobStatus Status,
    string? Salary
) : IRequest<JobDto>;

