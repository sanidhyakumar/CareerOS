namespace CareerOS.Application.Features.Profile.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}".Trim()
        : $"{FirstName} {MiddleName} {LastName}".Trim();
    public string? PreferredName { get; set; }
    public string? Headline { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? SecondaryEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }

    public List<ProfileCitizenshipDto> Citizenships { get; set; } = new();
    public List<WorkAuthorizationDto> WorkAuthorizations { get; set; } = new();
    public List<SocialLinkDto> SocialLinks { get; set; } = new();
    public List<AddressDto> Addresses { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

