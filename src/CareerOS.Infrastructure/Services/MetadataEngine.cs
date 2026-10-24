using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Common.Models;
using System.Security.Cryptography;

namespace CareerOS.Infrastructure.Services;

public class MetadataEngine : IMetadataEngine
{
    public async Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var initialPosition = stream.CanSeek ? stream.Position : 0;
        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);

        if (stream.CanSeek)
        {
            stream.Position = initialPosition;
        }

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public FileMetadata ExtractMetadata(string originalFileName, long length, string contentType, string sha256)
    {
        var cleanFileName = Path.GetFileName(originalFileName);
        var extension = Path.GetExtension(cleanFileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = GetDefaultMimeType(extension);
        }

        return new FileMetadata(
            cleanFileName,
            cleanFileName,
            extension.ToLowerInvariant(),
            contentType,
            length,
            sha256.ToLowerInvariant()
        );
    }

    private static string GetDefaultMimeType(string extension) => extension.ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".txt" => "text/plain",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".json" => "application/json",
        ".zip" => "application/zip",
        _ => "application/octet-stream"
    };
}

