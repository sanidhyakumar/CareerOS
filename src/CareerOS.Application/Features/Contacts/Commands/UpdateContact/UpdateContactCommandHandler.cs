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
        var contact = await _uow.Contacts.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        contact.Update(request.Name, request.Email, request.Phone, request.Company,
            request.JobTitle, request.LinkedIn, request.Notes);

        await _uow.Contacts.UpdateAsync(contact, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}

