namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class ResumeSubmissionDto
{
    public Guid Id { get; set; }
    public Guid ResumeProfileId { get; set; }
    public string? ProfileName { get; set; }
    public Guid ResumeVersionId { get; set; }
    public string? VersionName { get; set; }
    public int VersionNumber { get; set; }
    public Guid? CoverLetterId { get; set; }
    public string? CoverLetterTitle { get; set; }
    public Guid? PortfolioVersionId { get; set; }
    public string? PortfolioName { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public string Channel { get; set; } = "Company Website";
    public string Status { get; set; } = "Submitted";
    public string? Notes { get; set; }
    public Guid? SnapshotId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateResumeSubmissionCommandDto
{
    public Guid ResumeProfileId { get; set; }
    public Guid ResumeVersionId { get; set; }
    public Guid? CoverLetterId { get; set; }
    public Guid? PortfolioVersionId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public DateTime? SubmissionDate { get; set; }
    public string Channel { get; set; } = "Company Website";
    public string Status { get; set; } = "Submitted";
    public string? Notes { get; set; }
}

public class UpdateResumeSubmissionCommandDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public string Channel { get; set; } = "Company Website";
    public string Status { get; set; } = "Submitted";
    public Guid? CoverLetterId { get; set; }
    public Guid? PortfolioVersionId { get; set; }
    public string? Notes { get; set; }
}

public class SubmissionSnapshotDto
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTime SnapshotDate { get; set; }
    public string ResumeProfileName { get; set; } = string.Empty;
    public string ResumeVersionName { get; set; } = string.Empty;
    public int ResumeVersionNumber { get; set; }
    public string? ResumeDocumentSha256 { get; set; }
    public string? ResumeDocumentFileName { get; set; }
    public string? ResumeContentSnapshot { get; set; }
    public string? CoverLetterTitle { get; set; }
    public string? CoverLetterBodySnapshot { get; set; }
    public string? PortfolioName { get; set; }
    public string? PortfolioUrlSnapshot { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
}

