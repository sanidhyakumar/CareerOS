using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Commands.SnoozeReminder;

public record SnoozeReminderCommand(Guid Id, int Days) : IRequest;

public class SnoozeReminderCommandHandler : IRequestHandler<SnoozeReminderCommand>
{
    private readonly IUnitOfWork _uow;

    public SnoozeReminderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
    {
        var duration = TimeSpan.FromDays(Math.Max(1, request.Days));
        await _uow.Reminders.SnoozeAsync(request.Id, duration, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

