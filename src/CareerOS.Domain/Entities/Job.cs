using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Job : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;

    public Guid? CompanyId { get; set; }
    public Company? CompanyEntity { get; set; }

    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Saved;
    public JobPriority Priority { get; set; } = JobPriority.Medium;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public WorkplaceType WorkplaceType { get; set; } = WorkplaceType.Remote;
    public SeniorityLevel Seniority { get; set; } = SeniorityLevel.Senior;

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Salary { get; set; }
    public string? Equity { get; set; }

    public DateTime? Deadline { get; set; }
    public string? TechStack { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? InterviewAt { get; set; }

    public Job() { }

    public static Job Create(
        string title,
        string company,
        Guid? companyId = null,
        string? location = null,
        string? url = null,
        JobPriority priority = JobPriority.Medium,
        EmploymentType employmentType = EmploymentType.FullTime,
        WorkplaceType workplaceType = WorkplaceType.Remote,
        SeniorityLevel seniority = SeniorityLevel.Senior,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        string currency = "USD",
        string? salary = null,
        string? equity = null,
        string? department = null,
        string? techStack = null,
        DateTime? deadline = null,
        string? description = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(company)) throw new ArgumentException("Company is required.", nameof(company));

        string computedSalary = salary?.Trim() ?? FormatSalaryRange(minSalary, maxSalary, currency);

        return new Job
        {
            Title = title.Trim(),
            Company = company.Trim(),
            CompanyId = companyId,
            Location = location?.Trim(),
            Url = url?.Trim(),
            Priority = priority,
            EmploymentType = employmentType,
            WorkplaceType = workplaceType,
            Seniority = seniority,
            MinSalary = minSalary,
            MaxSalary = maxSalary,
            Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpper(),
            Salary = computedSalary,
            Equity = equity?.Trim(),
            Department = department?.Trim(),
            TechStack = techStack?.Trim(),
            Deadline = deadline,
            Description = description?.Trim(),
            Notes = notes?.Trim(),
            Status = JobStatus.Saved
        };
    }

    public void Update(
        string title,
        string company,
        Guid? companyId,
        string? department,
        string? location,
        string? url,
        string? description,
        string? notes,
        JobStatus status,
        JobPriority priority,
        EmploymentType employmentType,
        WorkplaceType workplaceType,
        SeniorityLevel seniority,
        decimal? minSalary,
        decimal? maxSalary,
        string currency,
        string? salary,
        string? equity,
        string? techStack,
        DateTime? deadline,
        DateTime? appliedAt,
        DateTime? interviewAt)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(company)) throw new ArgumentException("Company is required.", nameof(company));

        Title = title.Trim();
        Company = company.Trim();
        CompanyId = companyId;
        Department = department?.Trim();
        Location = location?.Trim();
        Url = url?.Trim();
        Description = description?.Trim();
        Notes = notes?.Trim();
        Status = status;
        Priority = priority;
        EmploymentType = employmentType;
        WorkplaceType = workplaceType;
        Seniority = seniority;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpper();
        Salary = salary?.Trim() ?? FormatSalaryRange(minSalary, maxSalary, Currency);
        Equity = equity?.Trim();
        TechStack = techStack?.Trim();
        Deadline = deadline;
        AppliedAt = appliedAt;
        InterviewAt = interviewAt;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string FormatSalaryRange(decimal? min, decimal? max, string currency)
    {
        if (!min.HasValue && !max.HasValue) return string.Empty;
        if (min.HasValue && max.HasValue) return $"${min.Value:N0} - ${max.Value:N0} {currency}";
        if (min.HasValue) return $"From ${min.Value:N0} {currency}";
        return $"Up to ${max!.Value:N0} {currency}";
    }
}
