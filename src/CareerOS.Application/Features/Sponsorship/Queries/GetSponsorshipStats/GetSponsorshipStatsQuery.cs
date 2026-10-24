using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Queries.GetSponsorshipStats;

public record GetSponsorshipStatsQuery : IRequest<SponsorshipStatsDto>;

public class GetSponsorshipStatsQueryHandler : IRequestHandler<GetSponsorshipStatsQuery, SponsorshipStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetSponsorshipStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<SponsorshipStatsDto> Handle(GetSponsorshipStatsQuery request, CancellationToken cancellationToken)
    {
        var cases = (await _uow.Sponsorship.GetAllAsync(cancellationToken: cancellationToken)).ToList();

        var verifiedEvidences = cases.SelectMany(c => c.EvidenceList).Count(e => e.IsVerified);
        var distinctCountries = cases.Select(c => c.CountryId).Distinct().Count();

        return new SponsorshipStatsDto
        {
            TotalCases = cases.Count,
            EligibleCount = cases.Count(c => c.Eligibility == SponsorshipEligibilityStatus.Eligible),
            ConditionallyEligibleCount = cases.Count(c => c.Eligibility == SponsorshipEligibilityStatus.ConditionallyEligible),
            OpenYearRoundCount = cases.Count(c => c.Availability == VisaAvailabilityStatus.OpenYearRound),
            CapSubjectLotteryCount = cases.Count(c => c.Availability == VisaAvailabilityStatus.CapSubjectLottery),
            LeadsToPRCount = cases.Count(c => c.LeadsToPermanentResidency),
            VerifiedEvidenceCount = verifiedEvidences,
            TotalCountries = distinctCountries
        };
    }
}

