namespace CareerOS.Application.Features.CareerProfiles.DTOs;

public class CareerProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Relationship { get; set; } = "Self";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string ColorCode { get; set; } = "#2563eb";
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CareerProfilePreferenceDto? Preferences { get; set; }
    public List<CareerProfileAddressDto> Addresses { get; set; } = new();

    // Summary counts
    public int DocumentCount { get; set; }
    public int ApplicationCount { get; set; }
    public int InterviewCount { get; set; }
    public int ContactCount { get; set; }
    public int SponsorshipCount { get; set; }
    public int WikiCount { get; set; }
    public int ResumeProfileCount { get; set; }
}

public class CareerProfilePreferenceDto
{
    public Guid Id { get; set; }
    public Guid CareerProfileId { get; set; }
    public string TargetRoles { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
    public decimal? TargetSalaryMin { get; set; }
    public decimal? TargetSalaryMax { get; set; }
    public string Currency { get; set; } = "USD";
    public bool OpenToRelocation { get; set; } = true;
    public string RemotePreference { get; set; } = "Remote";
    public string? Notes { get; set; }
}

public class CareerProfileAddressDto
{
    public Guid Id { get; set; }
    public Guid CareerProfileId { get; set; }
    public string Type { get; set; } = "Current";
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string StateOrProvince { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = true;
}

public class CreateCareerProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = "Self";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public string ColorCode { get; set; } = "#2563eb";
    public bool IsPrimary { get; set; }
    public CareerProfilePreferenceDto? Preferences { get; set; }
    public List<CareerProfileAddressDto>? Addresses { get; set; }
}

public class UpdateCareerProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = "Self";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public string ColorCode { get; set; } = "#2563eb";
    public bool IsPrimary { get; set; }
    public CareerProfilePreferenceDto? Preferences { get; set; }
    public List<CareerProfileAddressDto>? Addresses { get; set; }
}

public class FamilyDashboardDto
{
    public int TotalProfiles { get; set; }
    public int TotalApplications { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalInterviews { get; set; }
    public int TotalOffers { get; set; }
    public List<ProfileSummaryCardDto> ProfileSummaries { get; set; } = new();
    public List<FamilyActivityDto> RecentActivities { get; set; } = new();
}

public class ProfileSummaryCardDto
{
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public int ActiveApplications { get; set; }
    public int UpcomingInterviews { get; set; }
    public int TotalOffers { get; set; }
    public int ExpiringDocuments { get; set; }
    public string TargetRole { get; set; } = string.Empty;
}

public class FamilyActivityDto
{
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty; // Application, Interview, Document, Offer
}

public class ProfileAnalyticsDto
{
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int Interviewing { get; set; }
    public int Offers { get; set; }
    public int Rejections { get; set; }
    public double ConversionRate { get; set; }
    public int TotalDocuments { get; set; }
    public int ExpiringDocuments { get; set; }
    public int TotalContacts { get; set; }
}
