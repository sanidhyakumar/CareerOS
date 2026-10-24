using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Applications.Commands.DeleteApplication;

public record DeleteApplicationCommand(Guid Id) : IRequest<bool>;

public class DeleteApplicationCommandHandler : IRequestHandler<DeleteApplicationCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteApplicationCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _uow.Applications.GetByIdAsync(request.Id, includeDetails: false, cancellationToken: cancellationToken);
        if (application == null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        await _uow.Applications.DeleteAsync(application, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
