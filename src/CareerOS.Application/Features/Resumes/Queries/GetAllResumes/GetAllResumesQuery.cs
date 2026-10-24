using CareerOS.Application.Features.Resumes.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Queries.GetAllResumes;

public record GetAllResumesQuery : IRequest<IEnumerable<ResumeDto>>;

