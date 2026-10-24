using CareerOS.Application.Features.Contacts.DTOs;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetAllContacts;

public record GetAllContactsQuery : IRequest<IEnumerable<ContactDto>>;

