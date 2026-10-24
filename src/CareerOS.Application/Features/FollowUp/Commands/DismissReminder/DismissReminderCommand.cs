using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Commands.DismissReminder;

public record DismissReminderCommand(Guid Id) : IRequest;

public class DismissReminderCommandHandler : IRequestHandler<DismissReminderCommand>
{
    private readonly IUnitOfWork _uow;

    public DismissReminderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DismissReminderCommand request, CancellationToken cancellationToken)
    {
        await _uow.Reminders.DismissAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
