using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Contacts.OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Contacts.FindAsync([id], cancellationToken);

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
        var contact = await GetByIdAsync(id, cancellationToken);
        if (contact != null) _context.Contacts.Remove(contact);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Contacts.CountAsync(cancellationToken);
}

