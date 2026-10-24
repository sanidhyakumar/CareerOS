using CareerOS.Application.Features.Resumes.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.UpdateResume;

public record UpdateResumeCommand(Guid Id, string Name, string? Content, bool IsDefault, string? Tags) : IRequest<ResumeDto>;

