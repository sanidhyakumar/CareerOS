using CareerOS.Application.Features.Resumes.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.CreateResume;

public record CreateResumeCommand(string Name, string? Content, bool IsDefault, string? Tags) : IRequest<ResumeDto>;

