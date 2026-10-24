using AutoMapper;
using CareerOS.Application.Features.FollowUp.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Queries.GetReminders;

public record GetRemindersQuery(
    ReminderCategory? Category = null,
    ReminderSeverity? Severity = null,
    bool IncludeDismissed = false,
    bool IncludeSnoozed = false) : IRequest<List<ReminderDto>>;

public class GetRemindersQueryHandler : IRequestHandler<GetRemindersQuery, List<ReminderDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetRemindersQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<ReminderDto>> Handle(GetRemindersQuery request, CancellationToken cancellationToken)
    {
        // Automatically run detection scan if needed
        await _uow.Reminders.RunProactiveScanAsync(cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var items = await _uow.Reminders.GetAllAsync(
            category: request.Category,
            severity: request.Severity,
            includeDismissed: request.IncludeDismissed,
            includeSnoozed: request.IncludeSnoozed,
            cancellationToken: cancellationToken);

        return _mapper.Map<List<ReminderDto>>(items);
    }
}

