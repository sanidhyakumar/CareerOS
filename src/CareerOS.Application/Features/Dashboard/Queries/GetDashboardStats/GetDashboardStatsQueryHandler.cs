using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Dashboard.Queries.GetDashboardStats;

public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetDashboardStatsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var jobs = (await _uow.Jobs.GetAllAsync(cancellationToken: cancellationToken)).ToList();
        return new DashboardStatsDto
        {
            TotalJobs = jobs.Count,
            TotalContacts = await _uow.Contacts.CountAsync(cancellationToken),
            TotalResumes = await _uow.Resumes.CountAsync(cancellationToken),
            AppliedJobs = jobs.Count(j => j.Status == JobStatus.Applied),
            InterviewJobs = jobs.Count(j => j.Status == JobStatus.Interview || j.Status == JobStatus.PhoneScreen),
            OffersReceived = jobs.Count(j => j.Status == JobStatus.Offer || j.Status == JobStatus.Accepted)
        };
    }
}

