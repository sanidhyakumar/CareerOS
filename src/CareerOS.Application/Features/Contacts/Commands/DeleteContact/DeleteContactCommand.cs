using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.DeleteContact;

public record DeleteContactCommand(Guid Id) : IRequest;

