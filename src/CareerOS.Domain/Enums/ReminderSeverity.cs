namespace CareerOS.Domain.Enums;

public enum ReminderSeverity
{
    Low = 0,       // Informational / Advisory
    Medium = 1,    // Recommended Action
    High = 2,      // Urgent / Critical Deadline / 7+ days Stale
    Critical = 3   // Expired / Immediate Action Required
}
