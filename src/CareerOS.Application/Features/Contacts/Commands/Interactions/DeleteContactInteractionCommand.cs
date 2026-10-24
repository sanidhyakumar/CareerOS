using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.Interactions;

public record DeleteContactInteractionCommand(Guid InteractionId) : IRequest<bool>;

public class DeleteContactInteractionCommandHandler : IRequestHandler<DeleteContactInteractionCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteContactInteractionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteContactInteractionCommand request, CancellationToken cancellationToken)
    {
        await _uow.Contacts.DeleteInteractionAsync(request.InteractionId, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

