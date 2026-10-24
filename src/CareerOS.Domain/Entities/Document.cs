using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsLatestVersion { get; set; } = true;
    public string? Description { get; set; }

    // Phase 3 Document Vault Extensions
    public DocumentCategory Category { get; set; } = DocumentCategory.General;
    public string? DocumentNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? RootDocumentId { get; set; }
    public Document? RootDocument { get; set; }
    public ICollection<Document> ChildVersions { get; set; } = new List<Document>();

    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }

    public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    public ICollection<DocumentUsage> Usages { get; set; } = new List<DocumentUsage>();

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.UtcNow.Date;
    public bool IsExpiringSoon(int daysThreshold = 90) =>
        ExpiryDate.HasValue && !IsExpired && ExpiryDate.Value.Date <= DateTime.UtcNow.Date.AddDays(daysThreshold);
    public int? DaysUntilExpiry => ExpiryDate.HasValue
        ? (int)Math.Ceiling((ExpiryDate.Value.Date - DateTime.UtcNow.Date).TotalDays)
        : null;

    public Document() { }

    public static Document Create(
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSizeBytes,
        string storagePath,
        string sha256Hash,
        string? description = null,
        Guid? countryId = null,
        Guid? regionId = null,
        DocumentCategory category = DocumentCategory.General,
        string? documentNumber = null,
        string? issuingAuthority = null,
        DateTime? issueDate = null,
        DateTime? expiryDate = null)
    {
        return new Document
        {
            FileName = fileName,
            OriginalFileName = originalFileName,
            FileExtension = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}",
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            StoragePath = storagePath,
            Sha256Hash = sha256Hash.ToLowerInvariant(),
            Version = 1,
            IsLatestVersion = true,
            Description = description,
            CountryId = countryId,
            RegionId = regionId,
            Category = category,
            DocumentNumber = documentNumber?.Trim(),
            IssuingAuthority = issuingAuthority?.Trim(),
            IssueDate = issueDate,
            ExpiryDate = expiryDate
        };
    }

    public Document CreateNewVersion(
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSizeBytes,
        string storagePath,
        string sha256Hash,
        string? description = null,
        DocumentCategory? category = null,
        string? documentNumber = null,
        string? issuingAuthority = null,
        DateTime? issueDate = null,
        DateTime? expiryDate = null)
    {
        IsLatestVersion = false;
        UpdatedAt = DateTime.UtcNow;

        var rootId = RootDocumentId ?? Id;

        return new Document
        {
            RootDocumentId = rootId,
            Version = Version + 1,
            IsLatestVersion = true,
            FileName = fileName,
            OriginalFileName = originalFileName,
            FileExtension = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}",
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            StoragePath = storagePath,
            Sha256Hash = sha256Hash.ToLowerInvariant(),
            Description = description ?? Description,
            CountryId = CountryId,
            RegionId = RegionId,
            Category = category ?? Category,
            DocumentNumber = documentNumber?.Trim() ?? DocumentNumber,
            IssuingAuthority = issuingAuthority?.Trim() ?? IssuingAuthority,
            IssueDate = issueDate ?? IssueDate,
            ExpiryDate = expiryDate ?? ExpiryDate
        };
    }

    public void Move(string newStoragePath)
    {
        if (string.IsNullOrWhiteSpace(newStoragePath))
            throw new ArgumentException("New storage path cannot be empty.", nameof(newStoragePath));

        StoragePath = newStoragePath;
        UpdatedAt = DateTime.UtcNow;
    }
}
