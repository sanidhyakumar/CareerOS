using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllAsync(
        string? searchTerm = null,
        ContactCategory? category = null,
        Guid? companyId = null,
        bool? isStarred = null,
        CancellationToken cancellationToken = default);

    Task<Contact?> GetByIdAsync(Guid id, bool includeInteractions = true, CancellationToken cancellationToken = default);
    Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<ContactInteraction> AddInteractionAsync(ContactInteraction interaction, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactInteraction>> GetInteractionsAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task DeleteInteractionAsync(Guid interactionId, CancellationToken cancellationToken = default);
}
