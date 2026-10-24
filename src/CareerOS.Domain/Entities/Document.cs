using CareerOS.Domain.Common;

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

    public Guid? RootDocumentId { get; set; }
    public Document? RootDocument { get; set; }
    public ICollection<Document> ChildVersions { get; set; } = new List<Document>();

    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }

    public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();

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
        Guid? regionId = null)
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
            RegionId = regionId
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
        string? description = null)
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
            RegionId = RegionId
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

