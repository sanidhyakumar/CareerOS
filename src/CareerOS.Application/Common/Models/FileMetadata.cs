namespace CareerOS.Application.Common.Models;

public record FileMetadata(
    string FileName,
    string OriginalFileName,
    string FileExtension,
    string ContentType,
    long FileSizeBytes,
    string Sha256Hash
);

