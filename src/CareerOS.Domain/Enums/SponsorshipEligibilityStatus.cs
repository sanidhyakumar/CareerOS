namespace CareerOS.Domain.Enums;

public enum SponsorshipEligibilityStatus
{
    Eligible = 0,               // Candidate fulfills degree, skills, and prevailing wage conditions
    ConditionallyEligible = 1,  // Requires credential evaluation, salary adjustment, or role mapping
    PendingReview = 2,          // Under legal/immigration attorney review
    Ineligible = 3,             // Nationality or qualification restrictions
    Sponsored = 4               // Formal petition / sponsorship filed or approved
}

