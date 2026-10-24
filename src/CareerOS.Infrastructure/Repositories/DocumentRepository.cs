using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Document>> GetAllAsync(bool latestOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Documents
            .Include(d => d.Country)
            .Include(d => d.Region)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .AsQueryable();

        if (latestOnly)
        {
            query = query.Where(d => d.IsLatestVersion);
        }

        return await query.OrderByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Documents.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(d => d.Country)
                .Include(d => d.Region)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .Include(d => d.ChildVersions);
        }

        return await query.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.Documents.FindAsync([documentId], cancellationToken);
        if (doc == null) return Enumerable.Empty<Document>();

        var rootId = doc.RootDocumentId ?? doc.Id;

        return await _context.Documents
            .Where(d => d.Id == rootId || d.RootDocumentId == rootId)
            .OrderByDescending(d => d.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetBySha256Async(string sha256Hash, CancellationToken cancellationToken = default)
    {
        var normalized = sha256Hash.Trim().ToLowerInvariant();
        return await _context.Documents
            .Where(d => d.Sha256Hash == normalized)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Document> AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(document, cancellationToken);
        return document;
    }

    public Task UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        _context.Documents.Update(document);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _context.Documents.FindAsync([id], cancellationToken);
        if (document != null) _context.Documents.Remove(document);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Documents.CountAsync(cancellationToken);

    public async Task<IEnumerable<Document>> SearchAsync(
        string? searchTerm,
        Guid? countryId = null,
        Guid? regionId = null,
        Guid? tagId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Documents
            .Include(d => d.Country)
            .Include(d => d.Region)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(d =>
                d.OriginalFileName.ToLower().Contains(term) ||
                (d.Description != null && d.Description.ToLower().Contains(term)) ||
                d.Sha256Hash.Contains(term));
        }

        if (countryId.HasValue)
        {
            query = query.Where(d => d.CountryId == countryId.Value);
        }

        if (regionId.HasValue)
        {
            query = query.Where(d => d.RegionId == regionId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(d => d.DocumentTags.Any(dt => dt.TagId == tagId.Value));
        }

        return await query.OrderByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    }
}

