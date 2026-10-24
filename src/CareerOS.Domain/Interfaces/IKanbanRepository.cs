using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IKanbanRepository
{
    Task<KanbanCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<KanbanCard>> GetAllAsync(KanbanColumnType? column = null, CancellationToken cancellationToken = default);
    Task<KanbanCard> AddAsync(KanbanCard card, CancellationToken cancellationToken = default);
    Task UpdateAsync(KanbanCard card, CancellationToken cancellationToken = default);
    Task DeleteAsync(KanbanCard card, CancellationToken cancellationToken = default);
    Task ReorderCardsAsync(KanbanColumnType column, List<Guid> cardIdsInOrder, CancellationToken cancellationToken = default);
    Task SyncWithJobsAndApplicationsAsync(CancellationToken cancellationToken = default);
}
