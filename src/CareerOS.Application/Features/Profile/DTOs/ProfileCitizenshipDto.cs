namespace CareerOS.Application.Features.Profile.DTOs;

public class ProfileCitizenshipDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? PassportNumber { get; set; }
    public string? Notes { get; set; }
}

