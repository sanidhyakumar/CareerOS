using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetAllContacts;

public record GetAllContactsQuery(
    string? SearchTerm = null,
    ContactCategory? Category = null,
    Guid? CompanyId = null,
    bool? IsStarred = null
) : IRequest<IEnumerable<ContactDto>>;
