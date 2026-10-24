using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.FollowUp.Commands.TriggerFollowUpScan;

public record TriggerFollowUpScanCommand : IRequest<int>;

public class TriggerFollowUpScanCommandHandler : IRequestHandler<TriggerFollowUpScanCommand, int>
{
    private readonly IUnitOfWork _uow;

    public TriggerFollowUpScanCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(TriggerFollowUpScanCommand request, CancellationToken cancellationToken)
    {
        var count = await _uow.Reminders.RunProactiveScanAsync(cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return count;
    }
}

