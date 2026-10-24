namespace CareerOS.Domain.Enums;

public enum VisaAvailabilityStatus
{
    OpenYearRound = 0,         // Uncapped, continuous application windows
    CapSubjectLottery = 1,     // Annual statutory lottery window (e.g. US H-1B in March)
    QuotaWaitlist = 2,         // Quota currently exhausted for fiscal cycle
    ExpeditedProcessing = 3,   // Fast-track processing available (e.g. Canada GTS 2-week, US Premium)
    RestrictedOrPaused = 4     // Policy moratorium or restricted labor market
}

