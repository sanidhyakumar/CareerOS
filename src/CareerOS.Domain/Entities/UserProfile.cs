using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public string? Headline { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? SecondaryEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }

    public ICollection<ProfileCitizenship> Citizenships { get; set; } = new List<ProfileCitizenship>();
    public ICollection<WorkAuthorization> WorkAuthorizations { get; set; } = new List<WorkAuthorization>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public UserProfile() { }

    public static UserProfile Create(
        string firstName,
        string lastName,
        string email,
        string? headline = null,
        string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        return new UserProfile
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Headline = headline?.Trim(),
            PhoneNumber = phoneNumber?.Trim()
        };
    }

    public void UpdatePersonalInfo(
        string firstName,
        string? middleName,
        string lastName,
        string? preferredName,
        string? headline,
        string email,
        string? secondaryEmail,
        string? phoneNumber,
        string? secondaryPhone,
        string? bio,
        DateTime? dateOfBirth,
        string? avatarUrl)
    {
        FirstName = firstName.Trim();
        MiddleName = middleName?.Trim();
        LastName = lastName.Trim();
        PreferredName = preferredName?.Trim();
        Headline = headline?.Trim();
        Email = email.Trim().ToLowerInvariant();
        SecondaryEmail = secondaryEmail?.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber?.Trim();
        SecondaryPhone = secondaryPhone?.Trim();
        Bio = bio?.Trim();
        DateOfBirth = dateOfBirth;
        AvatarUrl = avatarUrl?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}

