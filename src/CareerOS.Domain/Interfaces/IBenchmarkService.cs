using CareerOS.Domain.Models;

namespace CareerOS.Domain.Interfaces;

public interface IBenchmarkService
{
    Task<BenchmarkSummary> ExecuteScaleBenchmarkAsync(
        int applicationCount = 10000,
        int documentCount = 50000,
        int searchRecordCount = 100000,
        bool cleanup = true,
        CancellationToken cancellationToken = default);

    Task<IDictionary<string, object>> GetSystemDiagnosticsAsync(CancellationToken cancellationToken = default);
}
