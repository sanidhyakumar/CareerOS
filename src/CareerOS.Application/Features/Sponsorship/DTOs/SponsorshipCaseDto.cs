using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Sponsorship.DTOs;

public class SponsorshipCaseDto
{
    public Guid Id { get; set; }

    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;

    public string ProgramName { get; set; } = string.Empty;
    public VisaProgramType ProgramType { get; set; }
    public string ProgramTypeName => ProgramType.ToString();

    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public Guid? JobId { get; set; }
    public string? JobTitle { get; set; }

    public SponsorshipEligibilityStatus Eligibility { get; set; }
    public string EligibilityName => Eligibility.ToString();

    public VisaAvailabilityStatus Availability { get; set; }
    public string AvailabilityName => Availability.ToString();

    public decimal? MinimumSalaryRequirement { get; set; }
    public string? Currency { get; set; }

    public DateTime? NextFiscalCycleDate { get; set; }
    public DateTime? FilingDeadline { get; set; }
    public int? EstimatedProcessingMonths { get; set; }

    public bool SupportsDualIntent { get; set; }
    public bool LeadsToPermanentResidency { get; set; }

    public string? Notes { get; set; }

    public List<SponsorshipEvidenceDto> EvidenceList { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SponsorshipEvidenceDto
{
    public Guid Id { get; set; }
    public Guid SponsorshipCaseId { get; set; }

    public Guid? DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public string? DocumentType { get; set; }

    public string EvidenceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SponsorshipStatsDto
{
    public int TotalCases { get; set; }
    public int EligibleCount { get; set; }
    public int ConditionallyEligibleCount { get; set; }
    public int OpenYearRoundCount { get; set; }
    public int CapSubjectLotteryCount { get; set; }
    public int LeadsToPRCount { get; set; }
    public int VerifiedEvidenceCount { get; set; }
    public int TotalCountries { get; set; }
}

