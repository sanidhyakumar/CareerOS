using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Commands.DeleteReminder;

public record DeleteReminderCommand(Guid Id) : IRequest;

public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteReminderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
    {
        var item = await _uow.Reminders.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(ReminderItem), request.Id);
        }

        await _uow.Reminders.DeleteAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

