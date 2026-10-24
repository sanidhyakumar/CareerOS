using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class SponsorshipCase : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public string ProgramName { get; set; } = string.Empty;
    public VisaProgramType ProgramType { get; set; } = VisaProgramType.Other;

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? JobId { get; set; }
    public Job? Job { get; set; }

    public SponsorshipEligibilityStatus Eligibility { get; set; } = SponsorshipEligibilityStatus.PendingReview;
    public VisaAvailabilityStatus Availability { get; set; } = VisaAvailabilityStatus.OpenYearRound;

    public decimal? MinimumSalaryRequirement { get; set; }
    public string? Currency { get; set; } = "USD";

    public DateTime? NextFiscalCycleDate { get; set; }
    public DateTime? FilingDeadline { get; set; }
    public int? EstimatedProcessingMonths { get; set; }

    public bool SupportsDualIntent { get; set; }
    public bool LeadsToPermanentResidency { get; set; }

    public string? Notes { get; set; }

    public ICollection<SponsorshipEvidence> EvidenceList { get; set; } = new List<SponsorshipEvidence>();

    public SponsorshipCase() { }

    public static SponsorshipCase Create(
        Guid countryId,
        string programName,
        VisaProgramType programType,
        SponsorshipEligibilityStatus eligibility = SponsorshipEligibilityStatus.PendingReview,
        VisaAvailabilityStatus availability = VisaAvailabilityStatus.OpenYearRound,
        Guid? companyId = null,
        Guid? jobId = null,
        decimal? minimumSalaryRequirement = null,
        string? currency = "USD",
        DateTime? nextFiscalCycleDate = null,
        DateTime? filingDeadline = null,
        int? estimatedProcessingMonths = null,
        bool supportsDualIntent = false,
        bool leadsToPermanentResidency = false,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(programName))
            throw new ArgumentException("Program name is required.", nameof(programName));

        return new SponsorshipCase
        {
            CountryId = countryId,
            ProgramName = programName.Trim(),
            ProgramType = programType,
            Eligibility = eligibility,
            Availability = availability,
            CompanyId = companyId,
            JobId = jobId,
            MinimumSalaryRequirement = minimumSalaryRequirement,
            Currency = currency?.Trim() ?? "USD",
            NextFiscalCycleDate = nextFiscalCycleDate,
            FilingDeadline = filingDeadline,
            EstimatedProcessingMonths = estimatedProcessingMonths,
            SupportsDualIntent = supportsDualIntent,
            LeadsToPermanentResidency = leadsToPermanentResidency,
            Notes = notes?.Trim()
        };
    }

    public void Update(
        Guid countryId,
        string programName,
        VisaProgramType programType,
        SponsorshipEligibilityStatus eligibility,
        VisaAvailabilityStatus availability,
        Guid? companyId,
        Guid? jobId,
        decimal? minimumSalaryRequirement,
        string? currency,
        DateTime? nextFiscalCycleDate,
        DateTime? filingDeadline,
        int? estimatedProcessingMonths,
        bool supportsDualIntent,
        bool leadsToPermanentResidency,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(programName))
            throw new ArgumentException("Program name is required.", nameof(programName));

        CountryId = countryId;
        ProgramName = programName.Trim();
        ProgramType = programType;
        Eligibility = eligibility;
        Availability = availability;
        CompanyId = companyId;
        JobId = jobId;
        MinimumSalaryRequirement = minimumSalaryRequirement;
        Currency = currency?.Trim() ?? "USD";
        NextFiscalCycleDate = nextFiscalCycleDate;
        FilingDeadline = filingDeadline;
        EstimatedProcessingMonths = estimatedProcessingMonths;
        SupportsDualIntent = supportsDualIntent;
        LeadsToPermanentResidency = leadsToPermanentResidency;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
