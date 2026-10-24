using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Queries.GetSponsorshipCaseById;

public record GetSponsorshipCaseByIdQuery(Guid Id) : IRequest<SponsorshipCaseDto>;

public class GetSponsorshipCaseByIdQueryHandler : IRequestHandler<GetSponsorshipCaseByIdQuery, SponsorshipCaseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetSponsorshipCaseByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SponsorshipCaseDto> Handle(GetSponsorshipCaseByIdQuery request, CancellationToken cancellationToken)
    {
        var sc = await _uow.Sponsorship.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken);
        if (sc == null)
        {
            throw new NotFoundException(nameof(SponsorshipCase), request.Id);
        }

        return _mapper.Map<SponsorshipCaseDto>(sc);
    }
}

