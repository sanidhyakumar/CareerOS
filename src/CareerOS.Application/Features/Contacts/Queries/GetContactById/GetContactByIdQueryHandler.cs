using AutoMapper;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetContactById;

public sealed class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetContactByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactDto?> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await _uow.Contacts.GetByIdAsync(request.Id, includeInteractions: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);
        return _mapper.Map<ContactDto>(contact);
    }
}

