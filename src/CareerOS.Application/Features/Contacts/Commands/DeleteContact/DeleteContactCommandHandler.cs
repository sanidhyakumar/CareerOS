using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Contacts.Commands.DeleteContact;

public sealed class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteContactCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _uow.Contacts.GetByIdAsync(request.Id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        await _uow.Contacts.DeleteAsync(contact.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

