using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Jobs.Commands.DeleteJob;

public sealed class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteJobCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _uow.Jobs.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Job", request.Id);

        await _uow.Jobs.DeleteAsync(job.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

