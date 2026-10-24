using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Kanban.Commands.DeleteKanbanCard;

public record DeleteKanbanCardCommand(Guid Id) : IRequest;

public class DeleteKanbanCardCommandHandler : IRequestHandler<DeleteKanbanCardCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteKanbanCardCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteKanbanCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _uow.Kanban.GetByIdAsync(request.Id, cancellationToken);
        if (card == null)
        {
            throw new NotFoundException(nameof(KanbanCard), request.Id);
        }

        await _uow.Kanban.DeleteAsync(card, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
