using AutoMapper;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery(bool ActiveOnly = false) : IRequest<IEnumerable<CountryDto>>;

public sealed class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCountriesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        var countries = await _uow.Countries.GetAllAsync(request.ActiveOnly, cancellationToken);
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }
}

