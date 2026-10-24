using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<JobApplication>> GetAllAsync(
        Guid? jobId = null,
        Guid? companyId = null,
        ApplicationStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default);
    Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default);
    Task DeleteAsync(JobApplication application, CancellationToken cancellationToken = default);
    Task<ApplicationTimelineEvent> AddTimelineEventAsync(ApplicationTimelineEvent timelineEvent, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationTimelineEvent>> GetTimelineEventsAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<ApplicationSnapshot?> GetSnapshotAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<ApplicationSnapshot> AddSnapshotAsync(ApplicationSnapshot snapshot, CancellationToken cancellationToken = default);
}
