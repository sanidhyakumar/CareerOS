using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Commands.DeleteSponsorshipCase;

public record DeleteSponsorshipCaseCommand(Guid Id) : IRequest;

public class DeleteSponsorshipCaseCommandHandler : IRequestHandler<DeleteSponsorshipCaseCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteSponsorshipCaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteSponsorshipCaseCommand request, CancellationToken cancellationToken)
    {
        var sponsorship = await _uow.Sponsorship.GetByIdAsync(request.Id, includeDetails: false, cancellationToken: cancellationToken);
        if (sponsorship == null)
        {
            throw new NotFoundException(nameof(SponsorshipCase), request.Id);
        }

        await _uow.Sponsorship.DeleteAsync(sponsorship, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

