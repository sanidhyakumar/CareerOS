using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ResumeVersion : BaseEntity
{
    public Guid ResumeProfileId { get; set; }
    public ResumeProfile ResumeProfile { get; set; } = null!;

    public string VersionName { get; set; } = string.Empty;
    public int VersionNumber { get; set; } = 1;

    public Guid? DocumentId { get; set; }
    public Document? Document { get; set; }

    public string FileFormat { get; set; } = "PDF"; // PDF, DOCX, Markdown
    public string? TargetCompany { get; set; }
    public string? TargetJobTitle { get; set; }
    public string? TailoredKeywords { get; set; }
    public string? Content { get; set; }
    public string? ChangeNotes { get; set; }
    public bool IsBaseVersion { get; set; }
    public bool IsArchived { get; set; }

    public ICollection<ResumeSubmission> Submissions { get; set; } = new List<ResumeSubmission>();

    public ResumeVersion() { }

    public static ResumeVersion Create(
        Guid resumeProfileId,
        string versionName,
        int versionNumber = 1,
        Guid? documentId = null,
        string fileFormat = "PDF",
        string? targetCompany = null,
        string? targetJobTitle = null,
        string? tailoredKeywords = null,
        string? content = null,
        string? changeNotes = null,
        bool isBaseVersion = false)
    {
        if (string.IsNullOrWhiteSpace(versionName)) throw new ArgumentException("Version name is required.", nameof(versionName));

        return new ResumeVersion
        {
            ResumeProfileId = resumeProfileId,
            VersionName = versionName.Trim(),
            VersionNumber = versionNumber,
            DocumentId = documentId,
            FileFormat = string.IsNullOrWhiteSpace(fileFormat) ? "PDF" : fileFormat.Trim(),
            TargetCompany = targetCompany?.Trim(),
            TargetJobTitle = targetJobTitle?.Trim(),
            TailoredKeywords = tailoredKeywords?.Trim(),
            Content = content?.Trim(),
            ChangeNotes = changeNotes?.Trim(),
            IsBaseVersion = isBaseVersion,
            IsArchived = false
        };
    }

    public void Update(
        string versionName,
        Guid? documentId,
        string fileFormat,
        string? targetCompany,
        string? targetJobTitle,
        string? tailoredKeywords,
        string? content,
        string? changeNotes,
        bool isBaseVersion,
        bool isArchived)
    {
        if (string.IsNullOrWhiteSpace(versionName)) throw new ArgumentException("Version name is required.", nameof(versionName));

        VersionName = versionName.Trim();
        DocumentId = documentId;
        FileFormat = string.IsNullOrWhiteSpace(fileFormat) ? "PDF" : fileFormat.Trim();
        TargetCompany = targetCompany?.Trim();
        TargetJobTitle = targetJobTitle?.Trim();
        TailoredKeywords = tailoredKeywords?.Trim();
        Content = content?.Trim();
        ChangeNotes = changeNotes?.Trim();
        IsBaseVersion = isBaseVersion;
        IsArchived = isArchived;
        UpdatedAt = DateTime.UtcNow;
    }
}

