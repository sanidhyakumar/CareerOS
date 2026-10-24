using AutoMapper;
using CareerOS.Application.Features.Kanban.DTOs;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Kanban.Queries.GetKanbanBoard;

public record GetKanbanBoardQuery : IRequest<KanbanBoardDto>;

public class GetKanbanBoardQueryHandler : IRequestHandler<GetKanbanBoardQuery, KanbanBoardDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetKanbanBoardQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<KanbanBoardDto> Handle(GetKanbanBoardQuery request, CancellationToken cancellationToken)
    {
        // 1. Auto-synchronize any newly added Jobs or Applications that aren't on the board yet
        await _uow.Kanban.SyncWithJobsAndApplicationsAsync(cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        // 2. Fetch all cards ordered by column and position
        var allCards = (await _uow.Kanban.GetAllAsync(cancellationToken: cancellationToken)).ToList();
        var cardDtos = _mapper.Map<List<KanbanCardDto>>(allCards);

        // 3. Group by the 6 standard Kanban columns
        var columnDefinitions = new[]
        {
            (KanbanColumnType.Interested, "Interested"),
            (KanbanColumnType.Saved, "Saved"),
            (KanbanColumnType.Applied, "Applied"),
            (KanbanColumnType.Interviewing, "Interviewing"),
            (KanbanColumnType.Offer, "Offer"),
            (KanbanColumnType.Rejected, "Rejected")
        };

        var columns = columnDefinitions.Select(col => new KanbanColumnDto
        {
            ColumnId = col.Item1,
            Title = col.Item2,
            Cards = cardDtos
                .Where(c => c.Column == col.Item1)
                .OrderBy(c => c.Position)
                .ThenByDescending(c => c.UpdatedAt)
                .ToList()
        }).ToList();

        return new KanbanBoardDto
        {
            Columns = columns,
            TotalCards = cardDtos.Count
        };
    }
}
