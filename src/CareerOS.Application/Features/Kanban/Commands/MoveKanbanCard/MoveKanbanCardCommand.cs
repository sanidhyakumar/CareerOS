using AutoMapper;
using CareerOS.Application.Features.Kanban.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Kanban.Commands.MoveKanbanCard;

public record MoveKanbanCardCommand(
    Guid CardId,
    KanbanColumnType TargetColumn,
    int NewPosition) : IRequest<KanbanCardDto>;

public class MoveKanbanCardCommandHandler : IRequestHandler<MoveKanbanCardCommand, KanbanCardDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MoveKanbanCardCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<KanbanCardDto> Handle(MoveKanbanCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _uow.Kanban.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
        {
            throw new NotFoundException(nameof(KanbanCard), request.CardId);
        }

        var sourceColumn = card.Column;
        card.MoveTo(request.TargetColumn, request.NewPosition);

        // Synchronize linked Job entity status if attached
        if (card.JobId.HasValue)
        {
            var job = await _uow.Jobs.GetByIdAsync(card.JobId.Value, cancellationToken);
            if (job != null)
            {
                var targetJobStatus = request.TargetColumn switch
                {
                    KanbanColumnType.Interested => JobStatus.Saved,
                    KanbanColumnType.Saved => JobStatus.Saved,
                    KanbanColumnType.Applied => JobStatus.Applied,
                    KanbanColumnType.Interviewing => JobStatus.Interview,
                    KanbanColumnType.Offer => JobStatus.Offer,
                    KanbanColumnType.Rejected => JobStatus.Rejected,
                    _ => job.Status
                };
                job.Status = targetJobStatus;
                job.UpdatedAt = DateTime.UtcNow;
                await _uow.Jobs.UpdateAsync(job, cancellationToken);
            }
        }

        // Synchronize linked Application entity status if attached
        if (card.ApplicationId.HasValue)
        {
            var app = await _uow.Applications.GetByIdAsync(card.ApplicationId.Value, includeDetails: false, cancellationToken: cancellationToken);
            if (app != null)
            {
                var targetAppStatus = request.TargetColumn switch
                {
                    KanbanColumnType.Applied => ApplicationStatus.Applied,
                    KanbanColumnType.Interviewing => ApplicationStatus.TechnicalInterview,
                    KanbanColumnType.Offer => ApplicationStatus.OfferReceived,
                    KanbanColumnType.Rejected => ApplicationStatus.Rejected,
                    _ => app.Status
                };

                if (app.Status != targetAppStatus)
                {
                    app.Status = targetAppStatus;
                    app.UpdatedAt = DateTime.UtcNow;
                    await _uow.Applications.UpdateAsync(app, cancellationToken);
                }
            }
        }

        await _uow.Kanban.UpdateAsync(card, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KanbanCardDto>(card);
    }
}
