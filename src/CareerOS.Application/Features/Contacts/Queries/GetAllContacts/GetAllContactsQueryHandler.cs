using AutoMapper;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Queries.GetAllContacts;

public sealed class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllContactsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        var contacts = await _uow.Contacts.GetAllAsync(
            searchTerm: request.SearchTerm,
            category: request.Category,
            companyId: request.CompanyId,
            isStarred: request.IsStarred,
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContactDto>>(contacts);
    }
}
