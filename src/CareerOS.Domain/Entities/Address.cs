using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public AddressType AddressType { get; set; } = AddressType.CurrentHome;
    public string Street1 { get; set; } = string.Empty;
    public string? Street2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;

    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }

    public string PostalCode { get; set; } = string.Empty;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public bool IsPrimary { get; set; }
    public bool WillingToRelocate { get; set; }
    public bool OpenToRemote { get; set; }

    public Address() { }

    public static Address Create(
        Guid userProfileId,
        AddressType addressType,
        string street1,
        string? street2,
        string city,
        string stateProvince,
        Guid? regionId,
        string postalCode,
        Guid countryId,
        bool isPrimary = false,
        bool willingToRelocate = false,
        bool openToRemote = true)
    {
        if (string.IsNullOrWhiteSpace(street1)) throw new ArgumentException("Street1 is required.", nameof(street1));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));

        return new Address
        {
            UserProfileId = userProfileId,
            AddressType = addressType,
            Street1 = street1.Trim(),
            Street2 = street2?.Trim(),
            City = city.Trim(),
            StateProvince = stateProvince?.Trim() ?? string.Empty,
            RegionId = regionId,
            PostalCode = postalCode?.Trim() ?? string.Empty,
            CountryId = countryId,
            IsPrimary = isPrimary,
            WillingToRelocate = willingToRelocate,
            OpenToRemote = openToRemote
        };
    }
}

