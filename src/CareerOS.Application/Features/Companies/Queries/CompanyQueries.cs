using AutoMapper;
using CareerOS.Application.Features.Companies.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Companies.Queries;

public record GetAllCompaniesQuery(
    string? SearchTerm = null,
    string? Industry = null,
    SponsorshipStatus? Sponsorship = null,
    RelocationStatus? Relocation = null,
    int? MinRating = null,
    bool? IsTarget = null) : IRequest<IReadOnlyList<CompanyDto>>;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, IReadOnlyList<CompanyDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCompaniesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _uow.Companies.GetAllAsync(
            request.SearchTerm,
            request.Industry,
            request.Sponsorship,
            request.Relocation,
            request.MinRating,
            request.IsTarget,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<CompanyDto>>(companies);
    }
}

public record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDto>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCompanyByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _uow.Companies.GetByIdAsync(request.Id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.Id);

        return _mapper.Map<CompanyDto>(company);
    }
}

public record GetCompanyStatsQuery : IRequest<CompanyStatsDto>;

public class GetCompanyStatsQueryHandler : IRequestHandler<GetCompanyStatsQuery, CompanyStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetCompanyStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<CompanyStatsDto> Handle(GetCompanyStatsQuery request, CancellationToken cancellationToken)
    {
        var all = await _uow.Companies.GetAllAsync(cancellationToken: cancellationToken);

        int total = all.Count;
        int targets = all.Count(c => c.IsTarget);
        int sponsoring = all.Count(c => c.SponsorshipStatus == SponsorshipStatus.Yes);
        int relocating = all.Count(c => c.RelocationStatus == RelocationStatus.Yes || c.RelocationStatus == RelocationStatus.DomesticOnly || c.RelocationStatus == RelocationStatus.International);

        var topIndustries = all
            .Where(c => !string.IsNullOrWhiteSpace(c.Industry))
            .GroupBy(c => c.Industry!)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();

        return new CompanyStatsDto
        {
            TotalCompanies = total,
            TargetCompanies = targets,
            SponsoringCompanies = sponsoring,
            RelocatingCompanies = relocating,
            TopIndustries = topIndustries
        };
    }
}

