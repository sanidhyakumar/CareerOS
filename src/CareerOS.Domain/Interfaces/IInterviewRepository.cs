using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IInterviewRepository
{
    Task<Interview?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<Interview>> GetAllAsync(
        Guid? applicationId = null,
        Guid? companyId = null,
        Guid? jobId = null,
        InterviewType? type = null,
        InterviewStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Interview>> GetUpcomingAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<Interview> AddAsync(Interview interview, CancellationToken cancellationToken = default);
    Task UpdateAsync(Interview interview, CancellationToken cancellationToken = default);
    Task DeleteAsync(Interview interview, CancellationToken cancellationToken = default);

    Task SetInterviewersAsync(Guid interviewId, IEnumerable<(Guid ContactId, string? Role)> interviewers, CancellationToken cancellationToken = default);
}

