using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Queries;

// Profiles
public record GetAllResumeProfilesQuery(bool IncludeArchived = false) : IRequest<IReadOnlyList<ResumeProfileDto>>;

public class GetAllResumeProfilesQueryHandler : IRequestHandler<GetAllResumeProfilesQuery, IReadOnlyList<ResumeProfileDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllResumeProfilesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeProfileDto>> Handle(GetAllResumeProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _uow.ResumeCenter.GetAllProfilesAsync(request.IncludeArchived, cancellationToken);
        return _mapper.Map<IReadOnlyList<ResumeProfileDto>>(profiles);
    }
}

public record GetResumeProfileByIdQuery(Guid Id) : IRequest<ResumeProfileDto>;

public class GetResumeProfileByIdQueryHandler : IRequestHandler<GetResumeProfileByIdQuery, ResumeProfileDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeProfileByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeProfileDto> Handle(GetResumeProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeProfile), request.Id);
        return _mapper.Map<ResumeProfileDto>(profile);
    }
}

// Versions
public record GetResumeVersionsByProfileQuery(Guid ProfileId, bool IncludeArchived = false) : IRequest<IReadOnlyList<ResumeVersionDto>>;

public class GetResumeVersionsByProfileQueryHandler : IRequestHandler<GetResumeVersionsByProfileQuery, IReadOnlyList<ResumeVersionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeVersionsByProfileQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeVersionDto>> Handle(GetResumeVersionsByProfileQuery request, CancellationToken cancellationToken)
    {
        var versions = await _uow.ResumeCenter.GetVersionsByProfileIdAsync(request.ProfileId, request.IncludeArchived, cancellationToken);
        return _mapper.Map<IReadOnlyList<ResumeVersionDto>>(versions);
    }
}

public record GetResumeVersionByIdQuery(Guid Id) : IRequest<ResumeVersionDto>;

public class GetResumeVersionByIdQueryHandler : IRequestHandler<GetResumeVersionByIdQuery, ResumeVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeVersionByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeVersionDto> Handle(GetResumeVersionByIdQuery request, CancellationToken cancellationToken)
    {
        var version = await _uow.ResumeCenter.GetVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeVersion), request.Id);
        return _mapper.Map<ResumeVersionDto>(version);
    }
}

// Cover Letters
public record GetAllCoverLettersQuery(Guid? ProfileId = null, bool TemplatesOnly = false) : IRequest<IReadOnlyList<CoverLetterDto>>;

public class GetAllCoverLettersQueryHandler : IRequestHandler<GetAllCoverLettersQuery, IReadOnlyList<CoverLetterDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCoverLettersQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CoverLetterDto>> Handle(GetAllCoverLettersQuery request, CancellationToken cancellationToken)
    {
        var letters = await _uow.ResumeCenter.GetAllCoverLettersAsync(request.ProfileId, request.TemplatesOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<CoverLetterDto>>(letters);
    }
}

public record GetCoverLetterByIdQuery(Guid Id) : IRequest<CoverLetterDto>;

public class GetCoverLetterByIdQueryHandler : IRequestHandler<GetCoverLetterByIdQuery, CoverLetterDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCoverLetterByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CoverLetterDto> Handle(GetCoverLetterByIdQuery request, CancellationToken cancellationToken)
    {
        var letter = await _uow.ResumeCenter.GetCoverLetterByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CoverLetter), request.Id);
        return _mapper.Map<CoverLetterDto>(letter);
    }
}

// Portfolio Versions
public record GetAllPortfolioVersionsQuery : IRequest<IReadOnlyList<PortfolioVersionDto>>;

public class GetAllPortfolioVersionsQueryHandler : IRequestHandler<GetAllPortfolioVersionsQuery, IReadOnlyList<PortfolioVersionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllPortfolioVersionsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PortfolioVersionDto>> Handle(GetAllPortfolioVersionsQuery request, CancellationToken cancellationToken)
    {
        var portfolios = await _uow.ResumeCenter.GetAllPortfolioVersionsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PortfolioVersionDto>>(portfolios);
    }
}

public record GetPortfolioVersionByIdQuery(Guid Id) : IRequest<PortfolioVersionDto>;

public class GetPortfolioVersionByIdQueryHandler : IRequestHandler<GetPortfolioVersionByIdQuery, PortfolioVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetPortfolioVersionByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PortfolioVersionDto> Handle(GetPortfolioVersionByIdQuery request, CancellationToken cancellationToken)
    {
        var portfolio = await _uow.ResumeCenter.GetPortfolioVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PortfolioVersion), request.Id);
        return _mapper.Map<PortfolioVersionDto>(portfolio);
    }
}

// Submissions
public record GetAllResumeSubmissionsQuery(Guid? ProfileId = null, Guid? VersionId = null) : IRequest<IReadOnlyList<ResumeSubmissionDto>>;

public class GetAllResumeSubmissionsQueryHandler : IRequestHandler<GetAllResumeSubmissionsQuery, IReadOnlyList<ResumeSubmissionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllResumeSubmissionsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeSubmissionDto>> Handle(GetAllResumeSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var submissions = await _uow.ResumeCenter.GetAllSubmissionsAsync(request.ProfileId, request.VersionId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ResumeSubmissionDto>>(submissions);
    }
}

public record GetResumeSubmissionByIdQuery(Guid Id) : IRequest<ResumeSubmissionDto>;

public class GetResumeSubmissionByIdQueryHandler : IRequestHandler<GetResumeSubmissionByIdQuery, ResumeSubmissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeSubmissionByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeSubmissionDto> Handle(GetResumeSubmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var submission = await _uow.ResumeCenter.GetSubmissionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeSubmission), request.Id);
        return _mapper.Map<ResumeSubmissionDto>(submission);
    }
}

// Snapshot
public record GetSubmissionSnapshotQuery(Guid SubmissionId) : IRequest<SubmissionSnapshotDto>;

public class GetSubmissionSnapshotQueryHandler : IRequestHandler<GetSubmissionSnapshotQuery, SubmissionSnapshotDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetSubmissionSnapshotQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SubmissionSnapshotDto> Handle(GetSubmissionSnapshotQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await _uow.ResumeCenter.GetSnapshotBySubmissionIdAsync(request.SubmissionId, cancellationToken)
            ?? throw new NotFoundException(nameof(SubmissionSnapshot), request.SubmissionId);
        return _mapper.Map<SubmissionSnapshotDto>(snapshot);
    }
}

// Analytics
public record GetResumeAnalyticsQuery : IRequest<ResumeAnalyticsDto>;

public class GetResumeAnalyticsQueryHandler : IRequestHandler<GetResumeAnalyticsQuery, ResumeAnalyticsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetResumeAnalyticsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeAnalyticsDto> Handle(GetResumeAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _uow.ResumeCenter.GetAllProfilesAsync(includeArchived: false, cancellationToken);
        var coverLetters = await _uow.ResumeCenter.GetAllCoverLettersAsync(null, false, cancellationToken);
        var portfolios = await _uow.ResumeCenter.GetAllPortfolioVersionsAsync(cancellationToken);
        var submissions = await _uow.ResumeCenter.GetAllSubmissionsAsync(null, null, cancellationToken);

        int totalVersions = profiles.Sum(p => p.Versions.Count);
        int totalSubmissions = submissions.Count;
        int totalInterviews = submissions.Count(s =>
            s.Status.Equals("Interviewing", StringComparison.OrdinalIgnoreCase) ||
            s.Status.Equals("Screening", StringComparison.OrdinalIgnoreCase) ||
            s.Status.Equals("Offer", StringComparison.OrdinalIgnoreCase));
        int totalOffers = submissions.Count(s => s.Status.Equals("Offer", StringComparison.OrdinalIgnoreCase));
        double overallResponseRate = totalSubmissions > 0
            ? Math.Round(((double)totalInterviews / totalSubmissions) * 100.0, 1)
            : 0.0;

        // Profile metrics
        var profileMetrics = profiles.Select(p =>
        {
            var pSubs = submissions.Where(s => s.ResumeProfileId == p.Id).ToList();
            int subCount = pSubs.Count;
            int intCount = pSubs.Count(s =>
                s.Status.Equals("Interviewing", StringComparison.OrdinalIgnoreCase) ||
                s.Status.Equals("Screening", StringComparison.OrdinalIgnoreCase) ||
                s.Status.Equals("Offer", StringComparison.OrdinalIgnoreCase));
            int offCount = pSubs.Count(s => s.Status.Equals("Offer", StringComparison.OrdinalIgnoreCase));
            double rate = subCount > 0 ? Math.Round(((double)intCount / subCount) * 100.0, 1) : 0.0;

            return new ProfileStrategyMetricDto
            {
                ProfileId = p.Id,
                ProfileName = p.Name,
                TargetRole = p.TargetRole,
                VersionsCount = p.Versions.Count,
                SubmissionsCount = subCount,
                InterviewsCount = intCount,
                OffersCount = offCount,
                ResponseRate = rate
            };
        }).OrderByDescending(m => m.SubmissionsCount).ToList();

        // Channel metrics
        var channelMetrics = submissions
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Channel) ? "Unknown" : s.Channel)
            .Select(g =>
            {
                int cSubs = g.Count();
                int cInts = g.Count(s =>
                    s.Status.Equals("Interviewing", StringComparison.OrdinalIgnoreCase) ||
                    s.Status.Equals("Screening", StringComparison.OrdinalIgnoreCase) ||
                    s.Status.Equals("Offer", StringComparison.OrdinalIgnoreCase));
                double cRate = cSubs > 0 ? Math.Round(((double)cInts / cSubs) * 100.0, 1) : 0.0;

                return new ChannelMetricDto
                {
                    Channel = g.Key,
                    SubmissionsCount = cSubs,
                    InterviewsCount = cInts,
                    ResponseRate = cRate
                };
            }).OrderByDescending(m => m.SubmissionsCount).ToList();

        var recentSubmissions = _mapper.Map<List<ResumeSubmissionDto>>(submissions.Take(10).ToList());

        return new ResumeAnalyticsDto
        {
            TotalProfiles = profiles.Count,
            TotalVersions = totalVersions,
            TotalCoverLetters = coverLetters.Count,
            TotalPortfolios = portfolios.Count,
            TotalSubmissions = totalSubmissions,
            TotalInterviews = totalInterviews,
            TotalOffers = totalOffers,
            OverallResponseRate = overallResponseRate,
            ProfileMetrics = profileMetrics,
            ChannelMetrics = channelMetrics,
            RecentSubmissions = recentSubmissions
        };
    }
}

