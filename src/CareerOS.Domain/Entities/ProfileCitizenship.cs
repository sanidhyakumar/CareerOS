using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ProfileCitizenship : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public bool IsPrimary { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? PassportNumber { get; set; }
    public string? Notes { get; set; }

    public ProfileCitizenship() { }

    public static ProfileCitizenship Create(
        Guid userProfileId,
        Guid countryId,
        bool isPrimary = false,
        DateTime? acquisitionDate = null,
        string? passportNumber = null,
        string? notes = null)
    {
        return new ProfileCitizenship
        {
            UserProfileId = userProfileId,
            CountryId = countryId,
            IsPrimary = isPrimary,
            AcquisitionDate = acquisitionDate,
            PassportNumber = passportNumber?.Trim(),
            Notes = notes?.Trim()
        };
    }
}

