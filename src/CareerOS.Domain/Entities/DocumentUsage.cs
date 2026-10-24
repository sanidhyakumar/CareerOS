using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class DocumentUsage : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? Purpose { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    public DocumentUsage() { }

    public static DocumentUsage Create(
        Guid documentId,
        string entityType,
        Guid entityId,
        string? entityName = null,
        string? purpose = null)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required.", nameof(entityType));

        return new DocumentUsage
        {
            DocumentId = documentId,
            EntityType = entityType.Trim(),
            EntityId = entityId,
            EntityName = entityName?.Trim(),
            Purpose = purpose?.Trim(),
            UsedAt = DateTime.UtcNow
        };
    }
}

