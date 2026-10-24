using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Queries.GetAllSponsorshipCases;

public record GetAllSponsorshipCasesQuery(
    Guid? CountryId = null,
    VisaProgramType? ProgramType = null,
    SponsorshipEligibilityStatus? Eligibility = null,
    VisaAvailabilityStatus? Availability = null,
    Guid? CompanyId = null,
    string? SearchTerm = null) : IRequest<List<SponsorshipCaseDto>>;

public class GetAllSponsorshipCasesQueryHandler : IRequestHandler<GetAllSponsorshipCasesQuery, List<SponsorshipCaseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllSponsorshipCasesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<SponsorshipCaseDto>> Handle(GetAllSponsorshipCasesQuery request, CancellationToken cancellationToken)
    {
        var cases = await _uow.Sponsorship.GetAllAsync(
            countryId: request.CountryId,
            programType: request.ProgramType,
            eligibility: request.Eligibility,
            availability: request.Availability,
            companyId: request.CompanyId,
            searchTerm: request.SearchTerm,
            cancellationToken: cancellationToken);

        return _mapper.Map<List<SponsorshipCaseDto>>(cases);
    }
}

