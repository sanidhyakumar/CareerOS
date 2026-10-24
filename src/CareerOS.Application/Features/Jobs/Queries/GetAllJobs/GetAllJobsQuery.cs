using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Queries.GetAllJobs;

public record GetAllJobsQuery(
    string? SearchTerm = null,
    Guid? CompanyId = null,
    JobStatus? Status = null,
    JobPriority? Priority = null,
    EmploymentType? EmploymentType = null,
    WorkplaceType? WorkplaceType = null,
    SeniorityLevel? Seniority = null
) : IRequest<IEnumerable<JobDto>>;
