using CareerOS.Application.Features.Applications.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Queries.GetApplicationStats;

public record GetApplicationStatsQuery : IRequest<ApplicationStatsDto>;

public class GetApplicationStatsQueryHandler : IRequestHandler<GetApplicationStatsQuery, ApplicationStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetApplicationStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ApplicationStatsDto> Handle(GetApplicationStatsQuery request, CancellationToken cancellationToken)
    {
        var applications = (await _uow.Applications.GetAllAsync(cancellationToken: cancellationToken)).ToList();

        int total = applications.Count;
        int active = applications.Count(a =>
            a.Status != ApplicationStatus.Rejected &&
            a.Status != ApplicationStatus.Withdrawn &&
            a.Status != ApplicationStatus.Accepted);

        int interviewing = applications.Count(a =>
            a.Status == ApplicationStatus.PhoneScreen ||
            a.Status == ApplicationStatus.TechnicalInterview ||
            a.Status == ApplicationStatus.BehavioralInterview ||
            a.Status == ApplicationStatus.FinalRound);

        int offers = applications.Count(a =>
            a.Status == ApplicationStatus.OfferReceived ||
            a.Status == ApplicationStatus.Accepted);

        int rejected = applications.Count(a => a.Status == ApplicationStatus.Rejected);

        decimal offerRate = total > 0
            ? Math.Round(((decimal)offers / total) * 100m, 1)
            : 0m;

        return new ApplicationStatsDto
        {
            TotalApplications = total,
            ActivePipeline = active,
            InterviewingCount = interviewing,
            OffersCount = offers,
            RejectedCount = rejected,
            OfferRate = offerRate
        };
    }
}

