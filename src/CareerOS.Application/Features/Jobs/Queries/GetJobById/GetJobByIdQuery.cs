using CareerOS.Application.Features.Jobs.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid Id) : IRequest<JobDto?>;

