using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Profile.DTOs;

public class WorkAuthorizationDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public WorkAuthorizationType AuthorizationType { get; set; }
    public string AuthorizationTypeName => AuthorizationType.ToString();
    public string? VisaType { get; set; }
    public bool RequiresSponsorship { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public Guid? DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public string? Notes { get; set; }
}

