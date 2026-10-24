namespace CareerOS.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream stream, string relativePath, CancellationToken cancellationToken = default);
    Task<Stream> ReadFileStreamAsync(string relativePath, CancellationToken cancellationToken = default);
    Task MoveFileAsync(string sourceRelativePath, string targetRelativePath, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
    bool FileExists(string relativePath);
    string GetFullPath(string relativePath);
}

