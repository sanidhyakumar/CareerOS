namespace CareerOS.Domain.Enums;

public enum ReminderCategory
{
    StaleApplication = 0,     // Applications submitted > 7 days ago with no updates
    ExpiringDocument = 1,     // Document expiring within 30-60 days or expired
    UpcomingInterview = 2,    // Interview scheduled within next 24-48 hours
    MissingAction = 3,        // Contact follow-up due, missing cover letter, or incomplete profile
    ContactFollowUp = 4,      // Recruiter or referral follow-up due
    General = 5
}
