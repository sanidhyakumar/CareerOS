using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class WorkAuthorization : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public WorkAuthorizationType AuthorizationType { get; set; } = WorkAuthorizationType.Citizen;
    public string? VisaType { get; set; }
    public bool RequiresSponsorship { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public Guid? DocumentId { get; set; }
    public Document? Document { get; set; }

    public string? Notes { get; set; }

    public WorkAuthorization() { }

    public static WorkAuthorization Create(
        Guid userProfileId,
        Guid countryId,
        WorkAuthorizationType authorizationType,
        string? visaType = null,
        bool requiresSponsorship = false,
        DateTime? validFrom = null,
        DateTime? validTo = null,
        Guid? documentId = null,
        string? notes = null)
    {
        return new WorkAuthorization
        {
            UserProfileId = userProfileId,
            CountryId = countryId,
            AuthorizationType = authorizationType,
            VisaType = visaType?.Trim(),
            RequiresSponsorship = requiresSponsorship,
            ValidFrom = validFrom,
            ValidTo = validTo,
            DocumentId = documentId,
            Notes = notes?.Trim()
        };
    }
}

