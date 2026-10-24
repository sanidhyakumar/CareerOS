namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class CoverLetterDto
{
    public Guid Id { get; set; }
    public Guid? ResumeProfileId { get; set; }
    public string? ProfileName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TargetCompany { get; set; }
    public string? TargetRole { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientTitle { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCoverLetterCommandDto
{
    public Guid? ResumeProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TargetCompany { get; set; }
    public string? TargetRole { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientTitle { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
}

public class UpdateCoverLetterCommandDto
{
    public Guid? ResumeProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TargetCompany { get; set; }
    public string? TargetRole { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientTitle { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
}

