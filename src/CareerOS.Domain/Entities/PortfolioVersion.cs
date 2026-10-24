using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class PortfolioVersion : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? TargetRole { get; set; }
    public string? Description { get; set; }
    public string? FeaturedProjects { get; set; } // JSON or text summary of featured projects
    public bool IsDefault { get; set; }

    public ICollection<ResumeSubmission> Submissions { get; set; } = new List<ResumeSubmission>();

    public PortfolioVersion() { }

    public static PortfolioVersion Create(
        string name,
        string? url = null,
        string? targetRole = null,
        string? description = null,
        string? featuredProjects = null,
        bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Portfolio name is required.", nameof(name));

        return new PortfolioVersion
        {
            Name = name.Trim(),
            Url = url?.Trim(),
            TargetRole = targetRole?.Trim(),
            Description = description?.Trim(),
            FeaturedProjects = featuredProjects?.Trim(),
            IsDefault = isDefault
        };
    }

    public void Update(
        string name,
        string? url,
        string? targetRole,
        string? description,
        string? featuredProjects,
        bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Portfolio name is required.", nameof(name));

        Name = name.Trim();
        Url = url?.Trim();
        TargetRole = targetRole?.Trim();
        Description = description?.Trim();
        FeaturedProjects = featuredProjects?.Trim();
        IsDefault = isDefault;
        UpdatedAt = DateTime.UtcNow;
    }
}

