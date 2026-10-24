using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class JobApplication : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? ResumeVersionId { get; set; }
    public ResumeVersion? ResumeVersion { get; set; }

    public Guid? CoverLetterId { get; set; }
    public CoverLetter? CoverLetter { get; set; }

    public Guid? PortfolioVersionId { get; set; }
    public PortfolioVersion? PortfolioVersion { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public string Channel { get; set; } = "Company Website"; // LinkedIn, Company Website, Referral, Job Board, Recruiter
    public Guid? ReferralContactId { get; set; }
    public Contact? ReferralContact { get; set; }
    public string? Notes { get; set; }

    public Guid? SnapshotId { get; set; }
    public ApplicationSnapshot? Snapshot { get; set; }

    public ICollection<ApplicationTimelineEvent> TimelineEvents { get; set; } = new List<ApplicationTimelineEvent>();

    public JobApplication() { }

    public static JobApplication Create(
        Guid jobId,
        Guid? companyId,
        Guid? resumeVersionId,
        Guid? coverLetterId,
        Guid? portfolioVersionId,
        DateTime applicationDate,
        ApplicationStatus status = ApplicationStatus.Applied,
        string channel = "Company Website",
        Guid? referralContactId = null,
        string? notes = null)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("JobId is required.", nameof(jobId));

        return new JobApplication
        {
            JobId = jobId,
            CompanyId = companyId,
            ResumeVersionId = resumeVersionId,
            CoverLetterId = coverLetterId,
            PortfolioVersionId = portfolioVersionId,
            ApplicationDate = applicationDate,
            Status = status,
            Channel = string.IsNullOrWhiteSpace(channel) ? "Company Website" : channel.Trim(),
            ReferralContactId = referralContactId,
            Notes = notes?.Trim()
        };
    }

    public void Update(
        Guid? resumeVersionId,
        Guid? coverLetterId,
        Guid? portfolioVersionId,
        DateTime applicationDate,
        ApplicationStatus status,
        string channel,
        Guid? referralContactId,
        string? notes)
    {
        ResumeVersionId = resumeVersionId;
        CoverLetterId = coverLetterId;
        PortfolioVersionId = portfolioVersionId;
        ApplicationDate = applicationDate;
        Status = status;
        Channel = string.IsNullOrWhiteSpace(channel) ? "Company Website" : channel.Trim();
        ReferralContactId = referralContactId;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ApplicationStatus newStatus, string? eventTitle = null, string? eventNotes = null)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        TimelineEvents.Add(new ApplicationTimelineEvent
        {
            JobApplicationId = Id,
            Status = newStatus,
            EventDate = DateTime.UtcNow,
            Title = string.IsNullOrWhiteSpace(eventTitle) ? $"Status changed to {newStatus}" : eventTitle.Trim(),
            Notes = eventNotes?.Trim()
        });
    }
}
