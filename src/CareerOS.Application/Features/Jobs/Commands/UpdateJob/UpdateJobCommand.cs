using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.UpdateJob;

public record UpdateJobCommand(
    Guid Id,
    string Title,
    string Company,
    Guid? CompanyId = null,
    string? Department = null,
    string? Location = null,
    string? Url = null,
    string? Description = null,
    string? Notes = null,
    JobStatus Status = JobStatus.Saved,
    JobPriority Priority = JobPriority.Medium,
    EmploymentType EmploymentType = EmploymentType.FullTime,
    WorkplaceType WorkplaceType = WorkplaceType.Remote,
    SeniorityLevel Seniority = SeniorityLevel.Senior,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    string Currency = "USD",
    string? Salary = null,
    string? Equity = null,
    string? TechStack = null,
    DateTime? Deadline = null,
    DateTime? AppliedAt = null,
    DateTime? InterviewAt = null
) : IRequest<JobDto>;
