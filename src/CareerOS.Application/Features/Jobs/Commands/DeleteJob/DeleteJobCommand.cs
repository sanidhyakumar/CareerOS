using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.DeleteJob;

public record DeleteJobCommand(Guid Id) : IRequest;

