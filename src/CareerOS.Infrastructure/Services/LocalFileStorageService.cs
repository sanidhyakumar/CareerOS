using CareerOS.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        var configPath = configuration["FileStorage:BasePath"];

        if (!string.IsNullOrWhiteSpace(configPath))
        {
            _baseStoragePath = Path.IsPathRooted(configPath)
                ? configPath
                : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), configPath));
        }
        else
        {
            // Default to data/storage in the current working directory
            _baseStoragePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "data", "storage"));
        }

        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
            _logger.LogInformation("[Storage] Initialized storage root at {Path}", _baseStoragePath);
        }
    }

    public string GetFullPath(string relativePath)
    {
        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        return Path.Combine(_baseStoragePath, normalized);
    }

    public bool FileExists(string relativePath)
    {
        var fullPath = GetFullPath(relativePath);
        return File.Exists(fullPath);
    }

    public async Task<string> SaveFileAsync(Stream stream, string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(relativePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);

        _logger.LogInformation("[Storage] Saved file to {FullPath}", fullPath);
        return relativePath.Replace('\\', '/');
    }

    public Task<Stream> ReadFileStreamAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(relativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File at '{relativePath}' was not found in storage.", fullPath);
        }

        Stream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        return Task.FromResult(fileStream);
    }

    public Task MoveFileAsync(string sourceRelativePath, string targetRelativePath, CancellationToken cancellationToken = default)
    {
        var sourceFullPath = GetFullPath(sourceRelativePath);
        var targetFullPath = GetFullPath(targetRelativePath);

        if (!File.Exists(sourceFullPath))
        {
            throw new FileNotFoundException($"Source file at '{sourceRelativePath}' was not found.", sourceFullPath);
        }

        var targetDir = Path.GetDirectoryName(targetFullPath);
        if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        File.Move(sourceFullPath, targetFullPath, overwrite: true);
        _logger.LogInformation("[Storage] Moved file from {Source} to {Target}", sourceFullPath, targetFullPath);

        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("[Storage] Deleted file at {FullPath}", fullPath);
        }

        return Task.CompletedTask;
    }
}

