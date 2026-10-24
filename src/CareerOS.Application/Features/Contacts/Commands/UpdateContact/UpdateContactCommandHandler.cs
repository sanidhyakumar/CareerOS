using AutoMapper;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.UpdateContact;

public sealed class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateContactCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _uow.Contacts.GetByIdAsync(request.Id, includeInteractions: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        contact.Update(
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
            tags: request.Tags,
            nextFollowUpDate: request.NextFollowUpDate);

        contact.Twitter = request.Twitter?.Trim();
        contact.JobId = request.JobId;

        if (request.CompanyId.HasValue && string.IsNullOrWhiteSpace(contact.Company))
        {
            var comp = await _uow.Companies.GetByIdAsync(request.CompanyId.Value, cancellationToken: cancellationToken);
            if (comp != null)
            {
                contact.Company = comp.Name;
            }
        }

        await _uow.Contacts.UpdateAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}
