using AutoMapper;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.CreateContact;

public sealed class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateContactCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = Contact.Create(
            name: request.Name,
            category: request.Category,
            email: request.Email,
            phone: request.Phone,
            company: request.Company,
            companyId: request.CompanyId,
            jobTitle: request.JobTitle,
            department: request.Department,
            linkedin: request.LinkedIn,
            location: request.Location,
            isStarred: request.IsStarred,
            notes: request.Notes,
            tags: request.Tags);

        contact.Twitter = request.Twitter?.Trim();
        contact.JobId = request.JobId;

        // Auto-resolve company name if companyId is provided but company string is empty
        if (request.CompanyId.HasValue && string.IsNullOrWhiteSpace(contact.Company))
        {
            var comp = await _uow.Companies.GetByIdAsync(request.CompanyId.Value, cancellationToken: cancellationToken);
            if (comp != null)
            {
                contact.Company = comp.Name;
            }
        }

        await _uow.Contacts.AddAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}
