using CareerOS.Application.Common.Interfaces;
using CareerOS.Application.Features.Analytics.DTOs;
using CareerOS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Application.Features.Analytics.Queries.GetCareerAnalytics;

public record GetCareerAnalyticsQuery(int? Days = null) : IRequest<CareerAnalyticsDto>;

public class GetCareerAnalyticsQueryHandler : IRequestHandler<GetCareerAnalyticsQuery, CareerAnalyticsDto>
{
    private readonly IApplicationDbContext _context;

    public GetCareerAnalyticsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CareerAnalyticsDto> Handle(GetCareerAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var applicationsQuery = _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Company)
            .Include(a => a.ResumeVersion)
                .ThenInclude(rv => rv != null ? rv.ResumeProfile : null)
            .Include(a => a.TimelineEvents)
            .AsNoTracking();

        if (request.Days.HasValue && request.Days.Value > 0)
        {
            var cutoff = DateTime.UtcNow.AddDays(-request.Days.Value);
            applicationsQuery = applicationsQuery.Where(a => a.ApplicationDate >= cutoff);
        }

        var applications = await applicationsQuery.ToListAsync(cancellationToken);
        var interviews = await _context.Interviews.AsNoTracking().ToListAsync(cancellationToken);
        var companies = await _context.Companies.AsNoTracking().ToListAsync(cancellationToken);
        var resumeVersions = await _context.ResumeVersions
            .Include(rv => rv.ResumeProfile)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int totalApps = applications.Count;

        // 1. Response Metrics
        int respondedCount = 0;
        int ghostedCount = 0;
        var responseDaysList = new List<double>();
        var now = DateTime.UtcNow;

        foreach (var app in applications)
        {
            bool hasProgressed = app.Status != ApplicationStatus.Draft &&
                                 app.Status != ApplicationStatus.Applied;

            var firstResponseEvent = app.TimelineEvents
                .Where(e => e.Status != ApplicationStatus.Applied && e.Status != ApplicationStatus.Draft)
                .OrderBy(e => e.EventDate)
                .FirstOrDefault();

            if (hasProgressed || firstResponseEvent != null)
            {
                respondedCount++;
                var responseDate = firstResponseEvent?.EventDate ?? app.UpdatedAt;
                var days = (responseDate - app.ApplicationDate).TotalDays;
                if (days >= 0) responseDaysList.Add(days);
            }
            else if (app.Status == ApplicationStatus.Applied && (now - app.ApplicationDate).TotalDays >= 14)
            {
                ghostedCount++;
            }
        }

        double responseRate = totalApps > 0 ? Math.Round((double)respondedCount / totalApps * 100, 1) : 0;
        double avgResponseDays = responseDaysList.Count > 0 ? Math.Round(responseDaysList.Average(), 1) : 0;

        var responseMetrics = new ResponseMetricsDto
        {
            TotalApplications = totalApps,
            RespondedCount = respondedCount,
            GhostedCount = ghostedCount,
            ResponseRatePercent = responseRate,
            AverageResponseDays = avgResponseDays
        };

        // 2. Application Funnel
        int appliedCount = applications.Count(a => a.Status != ApplicationStatus.Draft);
        int screenCount = applications.Count(a =>
            a.Status >= ApplicationStatus.PhoneScreen && a.Status != ApplicationStatus.Withdrawn);
        int techCount = applications.Count(a =>
            (a.Status >= ApplicationStatus.TechnicalInterview && a.Status != ApplicationStatus.Withdrawn));
        int finalCount = applications.Count(a =>
            (a.Status >= ApplicationStatus.FinalRound && a.Status != ApplicationStatus.Withdrawn));
        int offerCount = applications.Count(a =>
            a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted);
        int acceptedCount = applications.Count(a => a.Status == ApplicationStatus.Accepted);

        var funnel = new List<ApplicationFunnelStageDto>
        {
            new() { StageName = "Applied", Count = appliedCount, ConversionRatePercent = 100.0, DropoffCount = Math.Max(0, appliedCount - screenCount) },
            new() { StageName = "Phone Screen", Count = screenCount, ConversionRatePercent = appliedCount > 0 ? Math.Round((double)screenCount / appliedCount * 100, 1) : 0, DropoffCount = Math.Max(0, screenCount - techCount) },
            new() { StageName = "Technical Round", Count = techCount, ConversionRatePercent = screenCount > 0 ? Math.Round((double)techCount / screenCount * 100, 1) : 0, DropoffCount = Math.Max(0, techCount - finalCount) },
            new() { StageName = "Final Round", Count = finalCount, ConversionRatePercent = techCount > 0 ? Math.Round((double)finalCount / techCount * 100, 1) : 0, DropoffCount = Math.Max(0, finalCount - offerCount) },
            new() { StageName = "Offer Received", Count = offerCount, ConversionRatePercent = finalCount > 0 ? Math.Round((double)offerCount / finalCount * 100, 1) : 0, DropoffCount = Math.Max(0, offerCount - acceptedCount) },
            new() { StageName = "Offer Accepted", Count = acceptedCount, ConversionRatePercent = offerCount > 0 ? Math.Round((double)acceptedCount / offerCount * 100, 1) : 0, DropoffCount = 0 }
        };

        // 3. Interview Metrics
        int totalInterviews = interviews.Count;
        var completedInterviews = interviews.Where(i => i.Status == InterviewStatus.Completed || i.Outcome != InterviewOutcome.Pending).ToList();
        int completedCount = completedInterviews.Count;
        int passedCount = completedInterviews.Count(i => i.Outcome == InterviewOutcome.Passed || i.Outcome == InterviewOutcome.StrongHire || i.Outcome == InterviewOutcome.Hire);
        double passRate = completedCount > 0 ? Math.Round((double)passedCount / completedCount * 100, 1) : 0;

        var ratedInterviews = interviews.Where(i => i.Rating.HasValue && i.Rating.Value > 0).ToList();
        double avgRating = ratedInterviews.Count > 0 ? Math.Round(ratedInterviews.Average(i => i.Rating!.Value), 1) : 0;

        var roundsByType = interviews
            .GroupBy(i => i.InterviewType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var passRateByType = completedInterviews
            .GroupBy(i => i.InterviewType.ToString())
            .ToDictionary(
                g => g.Key,
                g => Math.Round((double)g.Count(i => i.Outcome == InterviewOutcome.Passed || i.Outcome == InterviewOutcome.StrongHire || i.Outcome == InterviewOutcome.Hire) / g.Count() * 100, 1)
            );

        var interviewMetrics = new InterviewMetricsDto
        {
            TotalInterviews = totalInterviews,
            CompletedInterviews = completedCount,
            PassRatePercent = passRate,
            AverageRating = avgRating,
            RoundsByType = roundsByType,
            PassRateByType = passRateByType
        };

        // 4. Offer Metrics
        var offerApps = applications.Where(a => a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted).ToList();
        var salaries = new List<decimal>();
        foreach (var oa in offerApps)
        {
            if (oa.Job != null)
            {
                if (oa.Job.MaxSalary.HasValue && oa.Job.MinSalary.HasValue)
                    salaries.Add((oa.Job.MinSalary.Value + oa.Job.MaxSalary.Value) / 2);
                else if (oa.Job.MaxSalary.HasValue)
                    salaries.Add(oa.Job.MaxSalary.Value);
                else if (oa.Job.MinSalary.HasValue)
                    salaries.Add(oa.Job.MinSalary.Value);
            }
        }

        var offerMetrics = new OfferMetricsDto
        {
            TotalOffers = offerCount,
            OfferRatePercent = totalApps > 0 ? Math.Round((double)offerCount / totalApps * 100, 1) : 0,
            AverageSalary = salaries.Count > 0 ? Math.Round(salaries.Average(), 0) : null,
            MinSalary = salaries.Count > 0 ? salaries.Min() : null,
            MaxSalary = salaries.Count > 0 ? salaries.Max() : null,
            Currency = offerApps.FirstOrDefault()?.Job?.Currency ?? "USD"
        };

        // 5. Sponsorship Metrics
        var sponsoringCompanyIds = companies
            .Where(c => c.SponsorshipStatus == SponsorshipStatus.Yes || c.SponsorshipStatus == SponsorshipStatus.CaseByCase)
            .Select(c => c.Id)
            .ToHashSet();

        int appsToSponsoring = applications.Count(a => a.CompanyId.HasValue && sponsoringCompanyIds.Contains(a.CompanyId.Value));
        int offersFromSponsoring = offerApps.Count(a => a.CompanyId.HasValue && sponsoringCompanyIds.Contains(a.CompanyId.Value));
        double sponsorshipOfferRate = appsToSponsoring > 0 ? Math.Round((double)offersFromSponsoring / appsToSponsoring * 100, 1) : 0;

        var policyDist = companies
            .GroupBy(c => c.SponsorshipStatus.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var sponsorshipMetrics = new SponsorshipAnalyticsDto
        {
            ApplicationsRequiringSponsorship = appsToSponsoring,
            ApplicationsDomestic = Math.Max(0, totalApps - appsToSponsoring),
            SponsoringEmployersCount = sponsoringCompanyIds.Count,
            NonSponsoringEmployersCount = companies.Count - sponsoringCompanyIds.Count,
            SponsorshipOfferRatePercent = sponsorshipOfferRate,
            PolicyDistribution = policyDist
        };

        // 6. Resume Effectiveness
        var resumeEffectiveness = new List<ResumeEffectivenessDto>();
        foreach (var rv in resumeVersions)
        {
            var versionApps = applications.Where(a => a.ResumeVersionId == rv.Id).ToList();
            int subCount = versionApps.Count;
            if (subCount == 0) continue;

            int intGen = versionApps.Count(a => a.Status >= ApplicationStatus.PhoneScreen && a.Status != ApplicationStatus.Withdrawn);
            int offGen = versionApps.Count(a => a.Status == ApplicationStatus.OfferReceived || a.Status == ApplicationStatus.Accepted);

            resumeEffectiveness.Add(new ResumeEffectivenessDto
            {
                VersionId = rv.Id,
                VersionName = rv.VersionName,
                ProfileName = rv.ResumeProfile?.Name ?? "General Profile",
                SubmissionsCount = subCount,
                InterviewsGenerated = intGen,
                OffersGenerated = offGen,
                InterviewRatePercent = Math.Round((double)intGen / subCount * 100, 1),
                OfferRatePercent = Math.Round((double)offGen / subCount * 100, 1)
            });
        }

        // 7. Rejection Analytics
        var rejectedApps = applications.Where(a => a.Status == ApplicationStatus.Rejected).ToList();
        int totalRejections = rejectedApps.Count;
        double rejectionRate = totalApps > 0 ? Math.Round((double)totalRejections / totalApps * 100, 1) : 0;

        var rejectionsByStage = new Dictionary<string, int>
        {
            { "Resume Screen", 0 },
            { "Phone Screen", 0 },
            { "Technical Interview", 0 },
            { "Final Round", 0 },
            { "Ghosted / Stale", ghostedCount }
        };

        var feedbackList = new List<string>();

        foreach (var rej in rejectedApps)
        {
            var priorEvent = rej.TimelineEvents
                .Where(e => e.Status != ApplicationStatus.Rejected)
                .OrderByDescending(e => e.EventDate)
                .FirstOrDefault();

            if (priorEvent != null)
            {
                if (priorEvent.Status == ApplicationStatus.FinalRound) rejectionsByStage["Final Round"]++;
                else if (priorEvent.Status == ApplicationStatus.TechnicalInterview) rejectionsByStage["Technical Interview"]++;
                else if (priorEvent.Status == ApplicationStatus.PhoneScreen) rejectionsByStage["Phone Screen"]++;
                else rejectionsByStage["Resume Screen"]++;
            }
            else
            {
                rejectionsByStage["Resume Screen"]++;
            }

            if (!string.IsNullOrWhiteSpace(rej.Notes))
            {
                feedbackList.Add(rej.Notes.Length > 80 ? rej.Notes.Substring(0, 80) + "..." : rej.Notes);
            }
        }

        var rejectionAnalytics = new RejectionAnalyticsDto
        {
            TotalRejections = totalRejections,
            RejectionRatePercent = rejectionRate,
            RejectionsByStage = rejectionsByStage,
            KeyFeedbackObservations = feedbackList.Take(5).ToList()
        };

        // 8. Velocity Timeline (Last 6 Months)
        var velocityTimeline = new List<ApplicationTimelinePointDto>();
        for (int i = 5; i >= 0; i--)
        {
            var monthDate = DateTime.UtcNow.AddMonths(-i);
            int m = monthDate.Month;
            int y = monthDate.Year;
            string label = monthDate.ToString("MMM yyyy");

            int appliedInMonth = applications.Count(a => a.ApplicationDate.Month == m && a.ApplicationDate.Year == y);
            int interviewsInMonth = interviews.Count(iv => iv.ScheduledAt.Month == m && iv.ScheduledAt.Year == y);
            int offersInMonth = offerApps.Count(a => a.UpdatedAt.Month == m && a.UpdatedAt.Year == y);

            velocityTimeline.Add(new ApplicationTimelinePointDto
            {
                MonthLabel = label,
                AppliedCount = appliedInMonth,
                InterviewCount = interviewsInMonth,
                OfferCount = offersInMonth
            });
        }

        return new CareerAnalyticsDto
        {
            ResponseMetrics = responseMetrics,
            Funnel = funnel,
            InterviewMetrics = interviewMetrics,
            OfferMetrics = offerMetrics,
            SponsorshipMetrics = sponsorshipMetrics,
            ResumeEffectiveness = resumeEffectiveness,
            RejectionAnalytics = rejectionAnalytics,
            VelocityTimeline = velocityTimeline
        };
    }
}
