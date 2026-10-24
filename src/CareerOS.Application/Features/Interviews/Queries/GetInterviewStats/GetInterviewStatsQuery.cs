using CareerOS.Application.Features.Interviews.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Queries.GetInterviewStats;

public record GetInterviewStatsQuery : IRequest<InterviewStatsDto>;

public class GetInterviewStatsQueryHandler : IRequestHandler<GetInterviewStatsQuery, InterviewStatsDto>
{
    private readonly IUnitOfWork _uow;

    public GetInterviewStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<InterviewStatsDto> Handle(GetInterviewStatsQuery request, CancellationToken cancellationToken)
    {
        var interviews = (await _uow.Interviews.GetAllAsync(cancellationToken: cancellationToken)).ToList();

        int total = interviews.Count;
        int upcoming = interviews.Count(i => i.Status == InterviewStatus.Scheduled && i.ScheduledAt >= DateTime.UtcNow);
        int completed = interviews.Count(i => i.Status == InterviewStatus.Completed);
        int tech = interviews.Count(i => i.InterviewType == InterviewType.Technical);
        int sysDesign = interviews.Count(i => i.InterviewType == InterviewType.SystemDesign);
        int behavioral = interviews.Count(i => i.InterviewType == InterviewType.Behavioral);

        int passed = interviews.Count(i =>
            i.Outcome == InterviewOutcome.Passed ||
            i.Outcome == InterviewOutcome.StrongHire ||
            i.Outcome == InterviewOutcome.Hire);

        int decided = interviews.Count(i =>
            i.Outcome != InterviewOutcome.Pending &&
            i.Outcome != InterviewOutcome.Neutral);

        decimal passRate = decided > 0
            ? Math.Round(((decimal)passed / decided) * 100m, 1)
            : 0m;

        return new InterviewStatsDto
        {
            TotalInterviews = total,
            UpcomingCount = upcoming,
            CompletedCount = completed,
            TechnicalCount = tech,
            SystemDesignCount = sysDesign,
            BehavioralCount = behavioral,
            PassedCount = passed,
            PassRate = passRate
        };
    }
}
