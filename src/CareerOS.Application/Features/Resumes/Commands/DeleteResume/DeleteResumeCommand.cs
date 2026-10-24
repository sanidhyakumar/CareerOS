using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.DeleteResume;

public record DeleteResumeCommand(Guid Id) : IRequest;

