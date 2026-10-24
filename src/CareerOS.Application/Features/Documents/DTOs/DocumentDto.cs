using CareerOS.Application.Features.Tags.DTOs;
using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Documents.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsLatestVersion { get; set; }
    public string? Description { get; set; }

    // Phase 3 Document Vault
    public DocumentCategory Category { get; set; }
    public string CategoryName => Category.ToString();
    public string? DocumentNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.UtcNow.Date;
    public bool IsExpiringSoon => ExpiryDate.HasValue && !IsExpired && ExpiryDate.Value.Date <= DateTime.UtcNow.Date.AddDays(90);
    public int? DaysUntilExpiry => ExpiryDate.HasValue ? (int)Math.Ceiling((ExpiryDate.Value.Date - DateTime.UtcNow.Date).TotalDays) : null;

    public Guid? RootDocumentId { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public Guid? RegionId { get; set; }
    public string? RegionName { get; set; }
    public List<TagDto> Tags { get; set; } = new();
    public List<DocumentUsageDto> Usages { get; set; } = new();
    public int UsageCount => Usages.Count;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DocumentVersionDto
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public bool IsLatestVersion { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Sha256Hash { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; }
    public string CategoryName => Category.ToString();
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DocumentUsageDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? Purpose { get; set; }
    public DateTime UsedAt { get; set; }
}
