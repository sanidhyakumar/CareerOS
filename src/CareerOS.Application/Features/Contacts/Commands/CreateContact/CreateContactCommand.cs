using CareerOS.Application.Features.Contacts.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    string Name,
    string? Email,
    string? Phone,
    string? Company,
    string? JobTitle,
    string? LinkedIn,
    string? Notes
) : IRequest<ContactDto>;

