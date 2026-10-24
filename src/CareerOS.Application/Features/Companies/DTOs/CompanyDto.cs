using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Companies.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Headquarters { get; set; }
    public string? Size { get; set; }
    public string? Stage { get; set; }
    public int? FoundedYear { get; set; }
    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? CareersUrl { get; set; }
    public string? TechBlogUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GlassdoorUrl { get; set; }

    public decimal? GlassdoorRating { get; set; }
    public int PersonalRating { get; set; }
    public int CultureRating { get; set; }
    public int WorkLifeBalanceRating { get; set; }

    public SponsorshipStatus SponsorshipStatus { get; set; }
    public string SponsorshipStatusName => SponsorshipStatus.ToString();
    public string? SponsoredVisas { get; set; }
    public string? SponsorshipNotes { get; set; }
    public RelocationStatus RelocationStatus { get; set; }
    public string RelocationStatusName => RelocationStatus.ToString();
    public string? RelocationNotes { get; set; }

    public string? Industry { get; set; }
    public string? Sector { get; set; }
    public string? Tags { get; set; }

    public string? Notes { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsTarget { get; set; }

    public int JobsCount { get; set; }
    public int ContactsCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCompanyCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Headquarters { get; set; }
    public string? Size { get; set; }
    public string? Stage { get; set; }
    public int? FoundedYear { get; set; }
    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? CareersUrl { get; set; }
    public string? TechBlogUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GlassdoorUrl { get; set; }

    public decimal? GlassdoorRating { get; set; }
    public int PersonalRating { get; set; } = 3;
    public int CultureRating { get; set; } = 3;
    public int WorkLifeBalanceRating { get; set; } = 3;

    public SponsorshipStatus SponsorshipStatus { get; set; } = SponsorshipStatus.Unknown;
    public string? SponsoredVisas { get; set; }
    public string? SponsorshipNotes { get; set; }
    public RelocationStatus RelocationStatus { get; set; } = RelocationStatus.Unknown;
    public string? RelocationNotes { get; set; }

    public string? Industry { get; set; }
    public string? Sector { get; set; }
    public string? Tags { get; set; }

    public string? Notes { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsTarget { get; set; }
}

public class UpdateCompanyCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Headquarters { get; set; }
    public string? Size { get; set; }
    public string? Stage { get; set; }
    public int? FoundedYear { get; set; }
    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? CareersUrl { get; set; }
    public string? TechBlogUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GlassdoorUrl { get; set; }

    public decimal? GlassdoorRating { get; set; }
    public int PersonalRating { get; set; } = 3;
    public int CultureRating { get; set; } = 3;
    public int WorkLifeBalanceRating { get; set; } = 3;

    public SponsorshipStatus SponsorshipStatus { get; set; } = SponsorshipStatus.Unknown;
    public string? SponsoredVisas { get; set; }
    public string? SponsorshipNotes { get; set; }
    public RelocationStatus RelocationStatus { get; set; } = RelocationStatus.Unknown;
    public string? RelocationNotes { get; set; }

    public string? Industry { get; set; }
    public string? Sector { get; set; }
    public string? Tags { get; set; }

    public string? Notes { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsTarget { get; set; }
}

public class CompanyStatsDto
{
    public int TotalCompanies { get; set; }
    public int TargetCompanies { get; set; }
    public int SponsoringCompanies { get; set; }
    public int RelocatingCompanies { get; set; }
    public List<string> TopIndustries { get; set; } = new();
}

