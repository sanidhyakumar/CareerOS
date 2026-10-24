using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Contact>> GetAllAsync(
        string? searchTerm = null,
        ContactCategory? category = null,
        Guid? companyId = null,
        bool? isStarred = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Contacts
            .Include(c => c.CompanyEntity)
            .Include(c => c.JobEntity)
            .Include(c => c.Interactions)
            .AsNoTracking();

        if (category.HasValue)
        {
            query = query.Where(c => c.Category == category.Value);
        }

        if (companyId.HasValue)
        {
            query = query.Where(c => c.CompanyId == companyId.Value);
        }

        if (isStarred.HasValue)
        {
            query = query.Where(c => c.IsStarred == isStarred.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Company != null && c.Company.ToLower().Contains(term)) ||
                (c.JobTitle != null && c.JobTitle.ToLower().Contains(term)) ||
                (c.Department != null && c.Department.ToLower().Contains(term)) ||
                (c.Notes != null && c.Notes.ToLower().Contains(term)) ||
                (c.Tags != null && c.Tags.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(c => c.IsStarred)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Contact?> GetByIdAsync(Guid id, bool includeInteractions = true, CancellationToken cancellationToken = default)
    {
        if (!includeInteractions)
        {
            return await _context.Contacts
                .Include(c => c.CompanyEntity)
                .Include(c => c.JobEntity)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        return await _context.Contacts
            .Include(c => c.CompanyEntity)
            .Include(c => c.JobEntity)
            .Include(c => c.Interactions)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        await _context.Contacts.AddAsync(contact, cancellationToken);
        return contact;
    }

    public Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _context.Contacts.Update(contact);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await _context.Contacts.FindAsync([id], cancellationToken);
        if (contact != null) _context.Contacts.Remove(contact);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Contacts.CountAsync(cancellationToken);

    public async Task<ContactInteraction> AddInteractionAsync(ContactInteraction interaction, CancellationToken cancellationToken = default)
    {
        await _context.ContactInteractions.AddAsync(interaction, cancellationToken);
        return interaction;
    }

    public async Task<IEnumerable<ContactInteraction>> GetInteractionsAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        return await _context.ContactInteractions
            .AsNoTracking()
            .Where(i => i.ContactId == contactId)
            .OrderByDescending(i => i.InteractionDate)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteInteractionAsync(Guid interactionId, CancellationToken cancellationToken = default)
    {
        var item = await _context.ContactInteractions.FindAsync([interactionId], cancellationToken);
        if (item != null)
        {
            _context.ContactInteractions.Remove(item);
        }
    }
}
