using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Applications.DTOs;

public class JobApplicationDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public string? JobDepartment { get; set; }
    public string? JobLocation { get; set; }
    public string? JobSalary { get; set; }
    public string? TechStack { get; set; }

    public Guid? ResumeVersionId { get; set; }
    public string? ResumeVersionName { get; set; }
    public string? ResumeProfileName { get; set; }

    public Guid? CoverLetterId { get; set; }
    public string? CoverLetterTitle { get; set; }

    public Guid? PortfolioVersionId { get; set; }
    public string? PortfolioName { get; set; }

    public ApplicationStatus Status { get; set; }
    public string StatusName => Status.ToString();

    public DateTime ApplicationDate { get; set; }
    public string Channel { get; set; } = "Company Website";

    public Guid? ReferralContactId { get; set; }
    public string? ReferralContactName { get; set; }

    public string? Notes { get; set; }

    public Guid? SnapshotId { get; set; }
    public bool HasSnapshot => SnapshotId.HasValue;

    public int TimelineEventsCount { get; set; }
    public string? LatestEventTitle { get; set; }
    public DateTime? LatestEventDate { get; set; }

    public List<ApplicationTimelineEventDto> TimelineEvents { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ApplicationTimelineEventDto
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public ApplicationStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApplicationSnapshotDto
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public DateTime SnapshotDate { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? WorkplaceType { get; set; }
    public string? Seniority { get; set; }
    public string? SalaryRange { get; set; }
    public string? TechStack { get; set; }

    public string? ResumeProfileName { get; set; }
    public string? ResumeVersionName { get; set; }
    public int? ResumeVersionNumber { get; set; }
    public string? ResumeDocumentSha256 { get; set; }
    public string? ResumeDocumentFileName { get; set; }
    public string? ResumeContentSnapshot { get; set; }

    public string? CoverLetterTitle { get; set; }
    public string? CoverLetterBodySnapshot { get; set; }

    public string? PortfolioName { get; set; }
    public string? PortfolioUrlSnapshot { get; set; }

    public string Channel { get; set; } = string.Empty;
    public string? ReferralContactName { get; set; }
    public string? SubmissionNotes { get; set; }
}

public class ApplicationStatsDto
{
    public int TotalApplications { get; set; }
    public int ActivePipeline { get; set; }
    public int InterviewingCount { get; set; }
    public int OffersCount { get; set; }
    public int RejectedCount { get; set; }
    public decimal OfferRate { get; set; }
}
