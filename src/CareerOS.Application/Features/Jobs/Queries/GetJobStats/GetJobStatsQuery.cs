using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Queries.GetJobStats;

public record GetJobStatsQuery : IRequest<JobStatsDto>;

public class GetJobStatsQueryHandler : IRequestHandler<GetJobStatsQuery, JobStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetJobStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<JobStatsDto> Handle(GetJobStatsQuery request, CancellationToken cancellationToken)
    {
        var allJobs = (await _uow.Jobs.GetAllAsync(cancellationToken: cancellationToken)).ToList();

        int total = allJobs.Count;
        int active = allJobs.Count(j => j.Status != JobStatus.Rejected && j.Status != JobStatus.Withdrawn);
        int dream = allJobs.Count(j => j.Priority == JobPriority.Dream);
        int high = allJobs.Count(j => j.Priority == JobPriority.High);
        int remote = allJobs.Count(j => j.WorkplaceType == WorkplaceType.Remote);

        var salaried = allJobs
            .Where(j => j.MinSalary.HasValue || j.MaxSalary.HasValue)
            .Select(j =>
            {
                var min = j.MinSalary ?? j.MaxSalary!.Value;
                var max = j.MaxSalary ?? j.MinSalary!.Value;
                return (min + max) / 2m;
            })
            .ToList();

        decimal? avgSalary = salaried.Any() ? Math.Round(salaried.Average(), 0) : null;

        return new JobStatsDto
        {
            TotalJobs = total,
            ActivePositions = active,
            DreamPriorityCount = dream,
            HighPriorityCount = high,
            RemoteCount = remote,
            AverageMidSalary = avgSalary
        };
    }
}
