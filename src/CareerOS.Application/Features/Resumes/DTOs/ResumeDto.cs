namespace CareerOS.Application.Features.Resumes.DTOs;

public class ResumeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Version { get; set; }
    public bool IsDefault { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

