using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class CareerProfile : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Relationship { get; set; } = "Self"; // Self, Spouse, Child, Family, Friend, Client, Other
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string ColorCode { get; set; } = "#2563eb"; // Visual color identifier for tabs, badges and cards
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Headline { get; set; }
    public string? Bio { get; set; }

    // Child configuration
    public CareerProfilePreference? Preferences { get; set; }
    public ICollection<CareerProfileAddress> Addresses { get; set; } = new List<CareerProfileAddress>();

    // Domain collections owned by this profile boundary
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<SponsorshipCase> SponsorshipCases { get; set; } = new List<SponsorshipCase>();
    public ICollection<WikiArticle> WikiArticles { get; set; } = new List<WikiArticle>();
    public ICollection<ResumeProfile> ResumeProfiles { get; set; } = new List<ResumeProfile>();
    public ICollection<CredentialItem> Credentials { get; set; } = new List<CredentialItem>();
    public ICollection<ReminderItem> Reminders { get; set; } = new List<ReminderItem>();

    public CareerProfile() { }

    public static CareerProfile Create(
        string name,
        string relationship = "Self",
        string email = "",
        string? phoneNumber = null,
        string? headline = null,
        string? colorCode = "#2563eb",
        bool isPrimary = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Profile name is required.", nameof(name));

        var slug = GenerateSlug(name);
        return new CareerProfile
        {
            Name = name.Trim(),
            Slug = slug,
            Relationship = string.IsNullOrWhiteSpace(relationship) ? "Self" : relationship.Trim(),
            Email = email?.Trim().ToLowerInvariant() ?? string.Empty,
            PhoneNumber = phoneNumber?.Trim(),
            Headline = headline?.Trim(),
            ColorCode = string.IsNullOrWhiteSpace(colorCode) ? "#2563eb" : colorCode.Trim(),
            IsPrimary = isPrimary,
            IsActive = true
        };
    }

    public static string GenerateSlug(string name)
    {
        var clean = name.ToLowerInvariant().Trim();
        clean = System.Text.RegularExpressions.Regex.Replace(clean, @"[^a-z0-9\s-]", "");
        clean = System.Text.RegularExpressions.Regex.Replace(clean, @"\s+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(clean) ? "profile-" + Guid.NewGuid().ToString("N")[..8] : clean;
    }
}
