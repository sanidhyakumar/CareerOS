using CareerOS.Application.Features.Contacts.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.UpdateContact;

public record UpdateContactCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Company,
    string? JobTitle,
    string? LinkedIn,
    string? Notes
) : IRequest<ContactDto>;

