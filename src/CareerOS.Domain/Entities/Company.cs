using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Company : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Headquarters { get; set; }
    public string? Size { get; set; }
    public string? Stage { get; set; }
    public int? FoundedYear { get; set; }
    public string? LogoUrl { get; set; }

    // Links & Social
    public string? WebsiteUrl { get; set; }
    public string? CareersUrl { get; set; }
    public string? TechBlogUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GlassdoorUrl { get; set; }

    // Ratings & Perception
    public decimal? GlassdoorRating { get; set; }
    public int PersonalRating { get; set; } = 3;
    public int CultureRating { get; set; } = 3;
    public int WorkLifeBalanceRating { get; set; } = 3;

    // Sponsorship & Relocation
    public SponsorshipStatus SponsorshipStatus { get; set; } = SponsorshipStatus.Unknown;
    public string? SponsoredVisas { get; set; }
    public string? SponsorshipNotes { get; set; }
    public RelocationStatus RelocationStatus { get; set; } = RelocationStatus.Unknown;
    public string? RelocationNotes { get; set; }

    // Domain & Categorization
    public string? Industry { get; set; }
    public string? Sector { get; set; }
    public string? Tags { get; set; }

    // Personal Assessment
    public string? Notes { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsTarget { get; set; }

    // Reusable Linkages
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public Company() { }

    public static Company Create(
        string name,
        string? description = null,
        string? headquarters = null,
        string? industry = null,
        string? websiteUrl = null,
        string? careersUrl = null,
        string? linkedInUrl = null,
        string? glassdoorUrl = null,
        decimal? glassdoorRating = null,
        SponsorshipStatus sponsorshipStatus = SponsorshipStatus.Unknown,
        string? sponsoredVisas = null,
        RelocationStatus relocationStatus = RelocationStatus.Unknown,
        int personalRating = 3,
        bool isTarget = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Company name is required.", nameof(name));

        return new Company
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Headquarters = headquarters?.Trim(),
            Industry = industry?.Trim(),
            WebsiteUrl = websiteUrl?.Trim(),
            CareersUrl = careersUrl?.Trim(),
            LinkedInUrl = linkedInUrl?.Trim(),
            GlassdoorUrl = glassdoorUrl?.Trim(),
            GlassdoorRating = glassdoorRating,
            SponsorshipStatus = sponsorshipStatus,
            SponsoredVisas = sponsoredVisas?.Trim(),
            RelocationStatus = relocationStatus,
            PersonalRating = Math.Clamp(personalRating, 1, 5),
            IsTarget = isTarget
        };
    }

    public void Update(
        string name,
        string? description,
        string? headquarters,
        string? size,
        string? stage,
        int? foundedYear,
        string? logoUrl,
        string? websiteUrl,
        string? careersUrl,
        string? techBlogUrl,
        string? linkedInUrl,
        string? glassdoorUrl,
        decimal? glassdoorRating,
        int personalRating,
        int cultureRating,
        int workLifeBalanceRating,
        SponsorshipStatus sponsorshipStatus,
        string? sponsoredVisas,
        string? sponsorshipNotes,
        RelocationStatus relocationStatus,
        string? relocationNotes,
        string? industry,
        string? sector,
        string? tags,
        string? notes,
        string? pros,
        string? cons,
        bool isTarget)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Company name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Headquarters = headquarters?.Trim();
        Size = size?.Trim();
        Stage = stage?.Trim();
        FoundedYear = foundedYear;
        LogoUrl = logoUrl?.Trim();
        WebsiteUrl = websiteUrl?.Trim();
        CareersUrl = careersUrl?.Trim();
        TechBlogUrl = techBlogUrl?.Trim();
        LinkedInUrl = linkedInUrl?.Trim();
        GlassdoorUrl = glassdoorUrl?.Trim();
        GlassdoorRating = glassdoorRating;
        PersonalRating = Math.Clamp(personalRating, 1, 5);
        CultureRating = Math.Clamp(cultureRating, 1, 5);
        WorkLifeBalanceRating = Math.Clamp(workLifeBalanceRating, 1, 5);
        SponsorshipStatus = sponsorshipStatus;
        SponsoredVisas = sponsoredVisas?.Trim();
        SponsorshipNotes = sponsorshipNotes?.Trim();
        RelocationStatus = relocationStatus;
        RelocationNotes = relocationNotes?.Trim();
        Industry = industry?.Trim();
        Sector = sector?.Trim();
        Tags = tags?.Trim();
        Notes = notes?.Trim();
        Pros = pros?.Trim();
        Cons = cons?.Trim();
        IsTarget = isTarget;
        UpdatedAt = DateTime.UtcNow;
    }
}

