namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class ResumeProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public string? TargetIndustry { get; set; }
    public string? Summary { get; set; }
    public string? PrimarySkills { get; set; }
    public string? SecondarySkills { get; set; }
    public bool IsDefault { get; set; }
    public bool IsArchived { get; set; }
    public int VersionCount { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ResumeVersionDto> Versions { get; set; } = new();
}

public class CreateResumeProfileCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public string? TargetIndustry { get; set; }
    public string? Summary { get; set; }
    public string? PrimarySkills { get; set; }
    public string? SecondarySkills { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateResumeProfileCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public string? TargetIndustry { get; set; }
    public string? Summary { get; set; }
    public string? PrimarySkills { get; set; }
    public string? SecondarySkills { get; set; }
    public bool IsDefault { get; set; }
    public bool IsArchived { get; set; }
}

