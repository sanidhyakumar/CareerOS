using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class SubmissionSnapshot : BaseEntity
{
    public Guid SubmissionId { get; set; }
    public ResumeSubmission Submission { get; set; } = null!;

    public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;

    // Frozen Resume Details
    public string ResumeProfileName { get; set; } = string.Empty;
    public string ResumeVersionName { get; set; } = string.Empty;
    public int ResumeVersionNumber { get; set; } = 1;
    public string? ResumeDocumentSha256 { get; set; }
    public string? ResumeDocumentFileName { get; set; }
    public string? ResumeContentSnapshot { get; set; }

    // Frozen Cover Letter Details
    public string? CoverLetterTitle { get; set; }
    public string? CoverLetterBodySnapshot { get; set; }

    // Frozen Portfolio Details
    public string? PortfolioName { get; set; }
    public string? PortfolioUrlSnapshot { get; set; }

    // Additional ATS / Strategy metadata
    public string? MetadataJson { get; set; }

    public SubmissionSnapshot() { }

    public static SubmissionSnapshot Create(
        Guid submissionId,
        string resumeProfileName,
        string resumeVersionName,
        int resumeVersionNumber,
        string? resumeDocumentSha256 = null,
        string? resumeDocumentFileName = null,
        string? resumeContentSnapshot = null,
        string? coverLetterTitle = null,
        string? coverLetterBodySnapshot = null,
        string? portfolioName = null,
        string? portfolioUrlSnapshot = null,
        string? metadataJson = null)
    {
        return new SubmissionSnapshot
        {
            SubmissionId = submissionId,
            SnapshotDate = DateTime.UtcNow,
            ResumeProfileName = resumeProfileName ?? string.Empty,
            ResumeVersionName = resumeVersionName ?? string.Empty,
            ResumeVersionNumber = resumeVersionNumber,
            ResumeDocumentSha256 = resumeDocumentSha256,
            ResumeDocumentFileName = resumeDocumentFileName,
            ResumeContentSnapshot = resumeContentSnapshot,
            CoverLetterTitle = coverLetterTitle,
            CoverLetterBodySnapshot = coverLetterBodySnapshot,
            PortfolioName = portfolioName,
            PortfolioUrlSnapshot = portfolioUrlSnapshot,
            MetadataJson = metadataJson
        };
    }
}

