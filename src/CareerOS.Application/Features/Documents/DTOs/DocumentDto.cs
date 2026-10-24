using CareerOS.Application.Features.Tags.DTOs;

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
    public Guid? RootDocumentId { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public Guid? RegionId { get; set; }
    public string? RegionName { get; set; }
    public List<TagDto> Tags { get; set; } = new();
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
    public DateTime CreatedAt { get; set; }
}

