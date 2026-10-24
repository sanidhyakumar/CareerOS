using CareerOS.Application.Features.Contacts.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetContactById;

public record GetContactByIdQuery(Guid Id) : IRequest<ContactDto?>;

