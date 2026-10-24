using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Jobs.DTOs;

public class JobDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }

    public JobStatus Status { get; set; }
    public string StatusName => Status.ToString();

    public JobPriority Priority { get; set; }
    public string PriorityName => Priority.ToString();

    public EmploymentType EmploymentType { get; set; }
    public string EmploymentTypeName => EmploymentType.ToString();

    public WorkplaceType WorkplaceType { get; set; }
    public string WorkplaceTypeName => WorkplaceType.ToString();

    public SeniorityLevel Seniority { get; set; }
    public string SeniorityName => Seniority.ToString();

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Salary { get; set; }
    public string? Equity { get; set; }

    public DateTime? Deadline { get; set; }
    public string? TechStack { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? InterviewAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class JobStatsDto
{
    public int TotalJobs { get; set; }
    public int ActivePositions { get; set; }
    public int DreamPriorityCount { get; set; }
    public int HighPriorityCount { get; set; }
    public int RemoteCount { get; set; }
    public decimal? AverageMidSalary { get; set; }
}
