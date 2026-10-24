using CareerOS.Application.Features.Resumes.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Queries.GetResumeById;

public record GetResumeByIdQuery(Guid Id) : IRequest<ResumeDto?>;

