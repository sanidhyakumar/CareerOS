using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class CareerProfilePreference : BaseEntity
{
    public Guid CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string TargetRoles { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
    public decimal? TargetSalaryMin { get; set; }
    public decimal? TargetSalaryMax { get; set; }
    public string Currency { get; set; } = "USD";
    public bool OpenToRelocation { get; set; } = true;
    public string RemotePreference { get; set; } = "Remote"; // Remote, Hybrid, Onsite, Any
    public string? Notes { get; set; }

    public CareerProfilePreference() { }

    public static CareerProfilePreference Create(
        Guid careerProfileId,
        string targetRoles,
        string targetLocations,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        string currency = "USD",
        bool openToRelocation = true,
        string remotePreference = "Remote",
        string? notes = null)
    {
        return new CareerProfilePreference
        {
            CareerProfileId = careerProfileId,
            TargetRoles = targetRoles?.Trim() ?? string.Empty,
            TargetLocations = targetLocations?.Trim() ?? string.Empty,
            TargetSalaryMin = minSalary,
            TargetSalaryMax = maxSalary,
            Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant(),
            OpenToRelocation = openToRelocation,
            RemotePreference = string.IsNullOrWhiteSpace(remotePreference) ? "Remote" : remotePreference.Trim(),
            Notes = notes?.Trim()
        };
    }
}
