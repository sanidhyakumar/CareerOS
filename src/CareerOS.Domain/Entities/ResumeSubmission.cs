using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ResumeSubmission : BaseEntity
{
    public Guid ResumeProfileId { get; set; }
    public ResumeProfile ResumeProfile { get; set; } = null!;

    public Guid ResumeVersionId { get; set; }
    public ResumeVersion ResumeVersion { get; set; } = null!;

    public Guid? CoverLetterId { get; set; }
    public CoverLetter? CoverLetter { get; set; }

    public Guid? PortfolioVersionId { get; set; }
    public PortfolioVersion? PortfolioVersion { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public string Channel { get; set; } = "Company Website"; // LinkedIn, Company Website, Referral, Job Board, Recruiter
    public string Status { get; set; } = "Submitted"; // Submitted, Viewed, Screening, Interviewing, Offer, Rejected, Withdrawn
    public string? Notes { get; set; }

    public Guid? SnapshotId { get; set; }
    public SubmissionSnapshot? Snapshot { get; set; }

    public ResumeSubmission() { }

    public static ResumeSubmission Create(
        Guid resumeProfileId,
        Guid resumeVersionId,
        string companyName,
        string roleTitle,
        DateTime submissionDate,
        string channel = "Company Website",
        string status = "Submitted",
        Guid? coverLetterId = null,
        Guid? portfolioVersionId = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(companyName)) throw new ArgumentException("Company name is required.", nameof(companyName));
        if (string.IsNullOrWhiteSpace(roleTitle)) throw new ArgumentException("Role title is required.", nameof(roleTitle));

        return new ResumeSubmission
        {
            ResumeProfileId = resumeProfileId,
            ResumeVersionId = resumeVersionId,
            CoverLetterId = coverLetterId,
            PortfolioVersionId = portfolioVersionId,
            CompanyName = companyName.Trim(),
            RoleTitle = roleTitle.Trim(),
            SubmissionDate = submissionDate,
            Channel = string.IsNullOrWhiteSpace(channel) ? "Company Website" : channel.Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? "Submitted" : status.Trim(),
            Notes = notes?.Trim()
        };
    }

    public void Update(
        string companyName,
        string roleTitle,
        DateTime submissionDate,
        string channel,
        string status,
        Guid? coverLetterId,
        Guid? portfolioVersionId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(companyName)) throw new ArgumentException("Company name is required.", nameof(companyName));
        if (string.IsNullOrWhiteSpace(roleTitle)) throw new ArgumentException("Role title is required.", nameof(roleTitle));

        CompanyName = companyName.Trim();
        RoleTitle = roleTitle.Trim();
        SubmissionDate = submissionDate;
        Channel = string.IsNullOrWhiteSpace(channel) ? "Company Website" : channel.Trim();
        Status = string.IsNullOrWhiteSpace(status) ? "Submitted" : status.Trim();
        CoverLetterId = coverLetterId;
        PortfolioVersionId = portfolioVersionId;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}

