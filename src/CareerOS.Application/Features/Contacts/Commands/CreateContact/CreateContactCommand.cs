using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Enums;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    string Name,
    ContactCategory Category = ContactCategory.Recruiter,
    string? Email = null,
    string? Phone = null,
    string? Company = null,
    Guid? CompanyId = null,
    string? JobTitle = null,
    string? Department = null,
    string? LinkedIn = null,
    string? Twitter = null,
    string? Location = null,
    bool IsStarred = false,
    string? Notes = null,
    string? Tags = null,
    Guid? JobId = null
) : IRequest<ContactDto>;
