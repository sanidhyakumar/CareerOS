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
        var contact = Contact.Create(request.Name, request.Email, request.Phone, request.Company);
        contact.JobTitle = request.JobTitle;
        contact.LinkedIn = request.LinkedIn;
        contact.Notes = request.Notes;

        await _uow.Contacts.AddAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}

