using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IReminderRepository
{
    Task<ReminderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReminderItem>> GetAllAsync(
        ReminderCategory? category = null,
        ReminderSeverity? severity = null,
        bool includeDismissed = false,
        bool includeSnoozed = false,
        CancellationToken cancellationToken = default);

    Task<ReminderItem> AddAsync(ReminderItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReminderItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(ReminderItem item, CancellationToken cancellationToken = default);

    Task<int> RunProactiveScanAsync(CancellationToken cancellationToken = default);
    Task DismissAsync(Guid id, CancellationToken cancellationToken = default);
    Task SnoozeAsync(Guid id, TimeSpan duration, CancellationToken cancellationToken = default);
}

