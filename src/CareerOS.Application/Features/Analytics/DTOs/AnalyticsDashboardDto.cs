namespace CareerOS.Application.Features.Analytics.DTOs;

public class ApplicationFunnelStageDto
{
    public string StageName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double ConversionRatePercent { get; set; }
    public int DropoffCount { get; set; }
}

public class ResponseMetricsDto
{
    public int TotalApplications { get; set; }
    public int RespondedCount { get; set; }
    public int GhostedCount { get; set; }
    public double ResponseRatePercent { get; set; }
    public double AverageResponseDays { get; set; }
}

public class InterviewMetricsDto
{
    public int TotalInterviews { get; set; }
    public int CompletedInterviews { get; set; }
    public double PassRatePercent { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<string, int> RoundsByType { get; set; } = new();
    public Dictionary<string, double> PassRateByType { get; set; } = new();
}

public class OfferMetricsDto
{
    public int TotalOffers { get; set; }
    public double OfferRatePercent { get; set; }
    public decimal? AverageSalary { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string Currency { get; set; } = "USD";
}

public class SponsorshipAnalyticsDto
{
    public int ApplicationsRequiringSponsorship { get; set; }
    public int ApplicationsDomestic { get; set; }
    public int SponsoringEmployersCount { get; set; }
    public int NonSponsoringEmployersCount { get; set; }
    public double SponsorshipOfferRatePercent { get; set; }
    public Dictionary<string, int> PolicyDistribution { get; set; } = new();
}

public class ResumeEffectivenessDto
{
    public Guid VersionId { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public int SubmissionsCount { get; set; }
    public int InterviewsGenerated { get; set; }
    public int OffersGenerated { get; set; }
    public double InterviewRatePercent { get; set; }
    public double OfferRatePercent { get; set; }
}

public class RejectionAnalyticsDto
{
    public int TotalRejections { get; set; }
    public double RejectionRatePercent { get; set; }
    public Dictionary<string, int> RejectionsByStage { get; set; } = new();
    public List<string> KeyFeedbackObservations { get; set; } = new();
}

public class ApplicationTimelinePointDto
{
    public string MonthLabel { get; set; } = string.Empty;
    public int AppliedCount { get; set; }
    public int InterviewCount { get; set; }
    public int OfferCount { get; set; }
}

public class CareerAnalyticsDto
{
    public ResponseMetricsDto ResponseMetrics { get; set; } = new();
    public List<ApplicationFunnelStageDto> Funnel { get; set; } = new();
    public InterviewMetricsDto InterviewMetrics { get; set; } = new();
    public OfferMetricsDto OfferMetrics { get; set; } = new();
    public SponsorshipAnalyticsDto SponsorshipMetrics { get; set; } = new();
    public List<ResumeEffectivenessDto> ResumeEffectiveness { get; set; } = new();
    public RejectionAnalyticsDto RejectionAnalytics { get; set; } = new();
    public List<ApplicationTimelinePointDto> VelocityTimeline { get; set; } = new();
}

