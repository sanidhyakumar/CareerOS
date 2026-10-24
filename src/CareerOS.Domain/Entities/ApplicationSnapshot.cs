using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ApplicationSnapshot : BaseEntity
{
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;

    // Frozen Job & Company Details
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? WorkplaceType { get; set; }
    public string? Seniority { get; set; }
    public string? SalaryRange { get; set; }
    public string? TechStack { get; set; }

    // Frozen Resume Details
    public string? ResumeProfileName { get; set; }
    public string? ResumeVersionName { get; set; }
    public int? ResumeVersionNumber { get; set; }
    public string? ResumeDocumentSha256 { get; set; }
    public string? ResumeDocumentFileName { get; set; }
    public string? ResumeContentSnapshot { get; set; }

    // Frozen Cover Letter Details
    public string? CoverLetterTitle { get; set; }
    public string? CoverLetterBodySnapshot { get; set; }

    // Frozen Portfolio Details
    public string? PortfolioName { get; set; }
    public string? PortfolioUrlSnapshot { get; set; }

    // Submission Context
    public string Channel { get; set; } = "Company Website";
    public string? ReferralContactName { get; set; }
    public string? SubmissionNotes { get; set; }

    public ApplicationSnapshot() { }

    public static ApplicationSnapshot Create(
        Guid jobApplicationId,
        string jobTitle,
        string companyName,
        string? department = null,
        string? location = null,
        string? workplaceType = null,
        string? seniority = null,
        string? salaryRange = null,
        string? techStack = null,
        string? resumeProfileName = null,
        string? resumeVersionName = null,
        int? resumeVersionNumber = null,
        string? resumeDocumentSha256 = null,
        string? resumeDocumentFileName = null,
        string? resumeContentSnapshot = null,
        string? coverLetterTitle = null,
        string? coverLetterBodySnapshot = null,
        string? portfolioName = null,
        string? portfolioUrlSnapshot = null,
        string channel = "Company Website",
        string? referralContactName = null,
        string? submissionNotes = null)
    {
        return new ApplicationSnapshot
        {
            JobApplicationId = jobApplicationId,
            SnapshotDate = DateTime.UtcNow,
            JobTitle = jobTitle ?? string.Empty,
            CompanyName = companyName ?? string.Empty,
            Department = department,
            Location = location,
            WorkplaceType = workplaceType,
            Seniority = seniority,
            SalaryRange = salaryRange,
            TechStack = techStack,
            ResumeProfileName = resumeProfileName,
            ResumeVersionName = resumeVersionName,
            ResumeVersionNumber = resumeVersionNumber,
            ResumeDocumentSha256 = resumeDocumentSha256,
            ResumeDocumentFileName = resumeDocumentFileName,
            ResumeContentSnapshot = resumeContentSnapshot,
            CoverLetterTitle = coverLetterTitle,
            CoverLetterBodySnapshot = coverLetterBodySnapshot,
            PortfolioName = portfolioName,
            PortfolioUrlSnapshot = portfolioUrlSnapshot,
            Channel = channel,
            ReferralContactName = referralContactName,
            SubmissionNotes = submissionNotes
        };
    }
}

