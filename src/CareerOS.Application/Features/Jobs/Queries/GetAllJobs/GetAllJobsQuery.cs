using CareerOS.Application.Features.Jobs.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Queries.GetAllJobs;

public record GetAllJobsQuery : IRequest<IEnumerable<JobDto>>;

