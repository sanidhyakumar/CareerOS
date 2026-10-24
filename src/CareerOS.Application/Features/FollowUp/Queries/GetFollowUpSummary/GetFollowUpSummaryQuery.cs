using CareerOS.Application.Features.FollowUp.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Queries.GetFollowUpSummary;

public record GetFollowUpSummaryQuery : IRequest<FollowUpSummaryDto>;

public class GetFollowUpSummaryQueryHandler : IRequestHandler<GetFollowUpSummaryQuery, FollowUpSummaryDto>
{
    private readonly IUnitOfWork _uow;

    public GetFollowUpSummaryQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<FollowUpSummaryDto> Handle(GetFollowUpSummaryQuery request, CancellationToken cancellationToken)
    {
        await _uow.Reminders.RunProactiveScanAsync(cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var activeReminders = (await _uow.Reminders.GetAllAsync(
            includeDismissed: false,
            includeSnoozed: false,
            cancellationToken: cancellationToken)).ToList();

        return new FollowUpSummaryDto
        {
            TotalActive = activeReminders.Count,
            StaleApplicationsCount = activeReminders.Count(r => r.Category == ReminderCategory.StaleApplication),
            ExpiringDocumentsCount = activeReminders.Count(r => r.Category == ReminderCategory.ExpiringDocument),
            UpcomingInterviewsCount = activeReminders.Count(r => r.Category == ReminderCategory.UpcomingInterview),
            MissingActionsCount = activeReminders.Count(r => r.Category == ReminderCategory.MissingAction),
            ContactFollowUpsCount = activeReminders.Count(r => r.Category == ReminderCategory.ContactFollowUp),
            HighOrCriticalCount = activeReminders.Count(r => r.Severity == ReminderSeverity.High || r.Severity == ReminderSeverity.Critical)
        };
    }
}
