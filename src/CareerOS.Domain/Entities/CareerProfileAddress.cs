using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class CareerProfileAddress : BaseEntity
{
    public Guid CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string Type { get; set; } = "Current"; // Current, Permanent, Work, Other
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string StateOrProvince { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = true;

    public CareerProfileAddress() { }

    public static CareerProfileAddress Create(
        Guid careerProfileId,
        string street,
        string city,
        string stateOrProvince,
        string postalCode,
        string country,
        string type = "Current",
        bool isPrimary = true)
    {
        return new CareerProfileAddress
        {
            CareerProfileId = careerProfileId,
            Street = street?.Trim() ?? string.Empty,
            City = city?.Trim() ?? string.Empty,
            StateOrProvince = stateOrProvince?.Trim() ?? string.Empty,
            PostalCode = postalCode?.Trim() ?? string.Empty,
            Country = country?.Trim() ?? string.Empty,
            Type = string.IsNullOrWhiteSpace(type) ? "Current" : type.Trim(),
            IsPrimary = isPrimary
        };
    }
}

