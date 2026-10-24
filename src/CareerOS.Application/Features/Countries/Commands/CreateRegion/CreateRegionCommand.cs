using AutoMapper;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Countries.Commands.CreateRegion;

public record CreateRegionCommand(Guid CountryId, string Name, string Code) : IRequest<RegionDto>;

public class CreateRegionCommandValidator : AbstractValidator<CreateRegionCommand>
{
    public CreateRegionCommandValidator()
    {
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}

public sealed class CreateRegionCommandHandler : IRequestHandler<CreateRegionCommand, RegionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateRegionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<RegionDto> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
    {
        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        var region = Region.Create(country.Id, request.Name, request.Code);
        await _uow.Regions.AddAsync(region, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RegionDto>(region);
    }
}

public record GetRegionsByCountryQuery(Guid CountryId) : IRequest<IEnumerable<RegionDto>>;

public sealed class GetRegionsByCountryQueryHandler : IRequestHandler<GetRegionsByCountryQuery, IEnumerable<RegionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetRegionsByCountryQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegionDto>> Handle(GetRegionsByCountryQuery request, CancellationToken cancellationToken)
    {
        var regions = await _uow.Regions.GetByCountryIdAsync(request.CountryId, cancellationToken);
        return _mapper.Map<IEnumerable<RegionDto>>(regions);
    }
}

