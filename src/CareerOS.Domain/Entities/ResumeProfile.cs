using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ResumeProfile : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public string? TargetIndustry { get; set; }
    public string? Summary { get; set; }
    public string? PrimarySkills { get; set; }
    public string? SecondarySkills { get; set; }
    public bool IsDefault { get; set; }
    public bool IsArchived { get; set; }

    public ICollection<ResumeVersion> Versions { get; set; } = new List<ResumeVersion>();
    public ICollection<CoverLetter> CoverLetters { get; set; } = new List<CoverLetter>();
    public ICollection<ResumeSubmission> Submissions { get; set; } = new List<ResumeSubmission>();

    public ResumeProfile() { }

    public static ResumeProfile Create(
        string name,
        string targetRole,
        string? targetIndustry = null,
        string? summary = null,
        string? primarySkills = null,
        string? secondarySkills = null,
        bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Profile name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(targetRole)) throw new ArgumentException("Target role is required.", nameof(targetRole));

        return new ResumeProfile
        {
            Name = name.Trim(),
            TargetRole = targetRole.Trim(),
            TargetIndustry = targetIndustry?.Trim(),
            Summary = summary?.Trim(),
            PrimarySkills = primarySkills?.Trim(),
            SecondarySkills = secondarySkills?.Trim(),
            IsDefault = isDefault,
            IsArchived = false
        };
    }

    public void Update(
        string name,
        string targetRole,
        string? targetIndustry,
        string? summary,
        string? primarySkills,
        string? secondarySkills,
        bool isDefault,
        bool isArchived)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Profile name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(targetRole)) throw new ArgumentException("Target role is required.", nameof(targetRole));

        Name = name.Trim();
        TargetRole = targetRole.Trim();
        TargetIndustry = targetIndustry?.Trim();
        Summary = summary?.Trim();
        PrimarySkills = primarySkills?.Trim();
        SecondarySkills = secondarySkills?.Trim();
        IsDefault = isDefault;
        IsArchived = isArchived;
        UpdatedAt = DateTime.UtcNow;
    }
}

