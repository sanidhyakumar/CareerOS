using CareerOS.Application.Common.Models;

namespace CareerOS.Application.Common.Interfaces;

public interface IMetadataEngine
{
    Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken = default);
    FileMetadata ExtractMetadata(string originalFileName, long length, string contentType, string sha256);
}

