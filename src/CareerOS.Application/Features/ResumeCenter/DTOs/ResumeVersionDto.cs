namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class ResumeVersionDto
{
    public Guid Id { get; set; }
    public Guid ResumeProfileId { get; set; }
    public string? ProfileName { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public int VersionNumber { get; set; } = 1;
    public Guid? DocumentId { get; set; }
    public string? DocumentFileName { get; set; }
    public string? DocumentSha256 { get; set; }
    public string FileFormat { get; set; } = "PDF";
    public string? TargetCompany { get; set; }
    public string? TargetJobTitle { get; set; }
    public string? TailoredKeywords { get; set; }
    public string? Content { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsBaseVersion { get; set; }
    public bool IsArchived { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateResumeVersionCommandDto
{
    public Guid ResumeProfileId { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public Guid? DocumentId { get; set; }
    public string FileFormat { get; set; } = "PDF";
    public string? TargetCompany { get; set; }
    public string? TargetJobTitle { get; set; }
    public string? TailoredKeywords { get; set; }
    public string? Content { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsBaseVersion { get; set; }
}

public class UpdateResumeVersionCommandDto
{
    public string VersionName { get; set; } = string.Empty;
    public Guid? DocumentId { get; set; }
    public string FileFormat { get; set; } = "PDF";
    public string? TargetCompany { get; set; }
    public string? TargetJobTitle { get; set; }
    public string? TailoredKeywords { get; set; }
    public string? Content { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsBaseVersion { get; set; }
    public bool IsArchived { get; set; }
}

