namespace CareerOS.Domain.Interfaces;

public interface IDataPortabilityService
{
    Task<byte[]> ExportFullBackupJsonAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ExportCsvAsync(string entityType, CancellationToken cancellationToken = default);
    Task<byte[]> ExportMarkdownWikiZipAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ExportFullSystemZipAsync(CancellationToken cancellationToken = default);
    Task<IDictionary<string, int>> ImportBackupJsonAsync(Stream jsonStream, bool overwriteExisting = false, CancellationToken cancellationToken = default);
}

