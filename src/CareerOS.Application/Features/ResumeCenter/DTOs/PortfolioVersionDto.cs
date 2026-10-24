namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class PortfolioVersionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? TargetRole { get; set; }
    public string? Description { get; set; }
    public string? FeaturedProjects { get; set; }
    public bool IsDefault { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePortfolioVersionCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? TargetRole { get; set; }
    public string? Description { get; set; }
    public string? FeaturedProjects { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdatePortfolioVersionCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? TargetRole { get; set; }
    public string? Description { get; set; }
    public string? FeaturedProjects { get; set; }
    public bool IsDefault { get; set; }
}

