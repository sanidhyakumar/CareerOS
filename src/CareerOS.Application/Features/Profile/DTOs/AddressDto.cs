using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Profile.DTOs;

public class AddressDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public AddressType AddressType { get; set; }
    public string AddressTypeName => AddressType.ToString();
    public string Street1 { get; set; } = string.Empty;
    public string? Street2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public Guid? RegionId { get; set; }
    public string? RegionName { get; set; }
    public string? RegionCode { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool WillingToRelocate { get; set; }
    public bool OpenToRemote { get; set; }
}

