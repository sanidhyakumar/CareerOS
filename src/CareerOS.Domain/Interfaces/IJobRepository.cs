using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync(
        string? searchTerm = null,
        Guid? companyId = null,
        JobStatus? status = null,
        JobPriority? priority = null,
        EmploymentType? employmentType = null,
        WorkplaceType? workplaceType = null,
        SeniorityLevel? seniority = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
