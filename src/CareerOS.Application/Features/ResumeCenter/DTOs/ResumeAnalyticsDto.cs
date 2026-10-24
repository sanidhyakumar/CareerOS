namespace CareerOS.Application.Features.ResumeCenter.DTOs;

public class ResumeAnalyticsDto
{
    public int TotalProfiles { get; set; }
    public int TotalVersions { get; set; }
    public int TotalCoverLetters { get; set; }
    public int TotalPortfolios { get; set; }
    public int TotalSubmissions { get; set; }
    public int TotalInterviews { get; set; }
    public int TotalOffers { get; set; }
    public double OverallResponseRate { get; set; }

    public List<ProfileStrategyMetricDto> ProfileMetrics { get; set; } = new();
    public List<ChannelMetricDto> ChannelMetrics { get; set; } = new();
    public List<ResumeSubmissionDto> RecentSubmissions { get; set; } = new();
}

public class ProfileStrategyMetricDto
{
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public int VersionsCount { get; set; }
    public int SubmissionsCount { get; set; }
    public int InterviewsCount { get; set; }
    public int OffersCount { get; set; }
    public double ResponseRate { get; set; }
}

public class ChannelMetricDto
{
    public string Channel { get; set; } = string.Empty;
    public int SubmissionsCount { get; set; }
    public int InterviewsCount { get; set; }
    public double ResponseRate { get; set; }
}

