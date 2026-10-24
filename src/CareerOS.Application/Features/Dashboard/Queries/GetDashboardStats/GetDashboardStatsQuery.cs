using MediatR;

namespace CareerOS.Application.Features.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class DashboardStatsDto
{
    public int TotalJobs { get; set; }
    public int TotalContacts { get; set; }
    public int TotalResumes { get; set; }
    public int AppliedJobs { get; set; }
    public int InterviewJobs { get; set; }
    public int OffersReceived { get; set; }
}

