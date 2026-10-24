using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class SponsorshipEvidence : BaseEntity
{
    public Guid SponsorshipCaseId { get; set; }
    public SponsorshipCase SponsorshipCase { get; set; } = null!;

    public Guid? DocumentId { get; set; }
    public Document? Document { get; set; }

    public string EvidenceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public SponsorshipEvidence() { }

    public static SponsorshipEvidence Create(
        Guid sponsorshipCaseId,
        string evidenceType,
        string description,
        Guid? documentId = null,
        bool isVerified = false,
        DateTime? expiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(evidenceType))
            throw new ArgumentException("Evidence type is required.", nameof(evidenceType));

        return new SponsorshipEvidence
        {
            SponsorshipCaseId = sponsorshipCaseId,
            EvidenceType = evidenceType.Trim(),
            Description = description?.Trim() ?? string.Empty,
            DocumentId = documentId,
            IsVerified = isVerified,
            ExpiryDate = expiryDate
        };
    }

    public void Update(string evidenceType, string description, Guid? documentId, bool isVerified, DateTime? expiryDate)
    {
        if (string.IsNullOrWhiteSpace(evidenceType))
            throw new ArgumentException("Evidence type is required.", nameof(evidenceType));

        EvidenceType = evidenceType.Trim();
        Description = description?.Trim() ?? string.Empty;
        DocumentId = documentId;
        IsVerified = isVerified;
        ExpiryDate = expiryDate;
        UpdatedAt = DateTime.UtcNow;
    }
}

