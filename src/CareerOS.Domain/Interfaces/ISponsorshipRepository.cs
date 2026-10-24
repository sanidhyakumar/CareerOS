using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Interfaces;

public interface ISponsorshipRepository
{
    Task<SponsorshipCase?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<SponsorshipCase>> GetAllAsync(
        Guid? countryId = null,
        VisaProgramType? programType = null,
        SponsorshipEligibilityStatus? eligibility = null,
        VisaAvailabilityStatus? availability = null,
        Guid? companyId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<SponsorshipCase> AddAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default);
    Task UpdateAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default);
    Task DeleteAsync(SponsorshipCase sponsorshipCase, CancellationToken cancellationToken = default);

    Task<SponsorshipEvidence?> GetEvidenceByIdAsync(Guid evidenceId, CancellationToken cancellationToken = default);
    Task<SponsorshipEvidence> AddEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default);
    Task UpdateEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default);
    Task DeleteEvidenceAsync(SponsorshipEvidence evidence, CancellationToken cancellationToken = default);
}

