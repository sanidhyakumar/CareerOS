using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class CoverLetter : BaseEntity
{
    public Guid? ResumeProfileId { get; set; }
    public ResumeProfile? ResumeProfile { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? TargetCompany { get; set; }
    public string? TargetRole { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientTitle { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }

    public ICollection<ResumeSubmission> Submissions { get; set; } = new List<ResumeSubmission>();

    public CoverLetter() { }

    public static CoverLetter Create(
        string title,
        string body,
        Guid? resumeProfileId = null,
        string? targetCompany = null,
        string? targetRole = null,
        string? recipientName = null,
        string? recipientTitle = null,
        bool isTemplate = false)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required.", nameof(body));

        return new CoverLetter
        {
            Title = title.Trim(),
            Body = body,
            ResumeProfileId = resumeProfileId,
            TargetCompany = targetCompany?.Trim(),
            TargetRole = targetRole?.Trim(),
            RecipientName = recipientName?.Trim(),
            RecipientTitle = recipientTitle?.Trim(),
            IsTemplate = isTemplate
        };
    }

    public void Update(
        string title,
        string body,
        Guid? resumeProfileId,
        string? targetCompany,
        string? targetRole,
        string? recipientName,
        string? recipientTitle,
        bool isTemplate)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required.", nameof(body));

        Title = title.Trim();
        Body = body;
        ResumeProfileId = resumeProfileId;
        TargetCompany = targetCompany?.Trim();
        TargetRole = targetRole?.Trim();
        RecipientName = recipientName?.Trim();
        RecipientTitle = recipientTitle?.Trim();
        IsTemplate = isTemplate;
        UpdatedAt = DateTime.UtcNow;
    }
}

