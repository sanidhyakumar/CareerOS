using CareerOS.Domain.Entities;

namespace CareerOS.Domain.Interfaces;

public interface IResumeCenterRepository
{
    // Profiles
    Task<IReadOnlyList<ResumeProfile>> GetAllProfilesAsync(bool includeArchived = false, CancellationToken cancellationToken = default);
    Task<ResumeProfile?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResumeProfile> AddProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default);
    Task DeleteProfileAsync(ResumeProfile profile, CancellationToken cancellationToken = default);
    Task SetDefaultProfileAsync(Guid profileId, CancellationToken cancellationToken = default);

    // Versions
    Task<IReadOnlyList<ResumeVersion>> GetVersionsByProfileIdAsync(Guid profileId, bool includeArchived = false, CancellationToken cancellationToken = default);
    Task<ResumeVersion?> GetVersionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResumeVersion> AddVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default);
    Task UpdateVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default);
    Task DeleteVersionAsync(ResumeVersion version, CancellationToken cancellationToken = default);
    Task<int> GetNextVersionNumberAsync(Guid profileId, CancellationToken cancellationToken = default);

    // Cover Letters
    Task<IReadOnlyList<CoverLetter>> GetAllCoverLettersAsync(Guid? profileId = null, bool templatesOnly = false, CancellationToken cancellationToken = default);
    Task<CoverLetter?> GetCoverLetterByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CoverLetter> AddCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default);
    Task UpdateCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default);
    Task DeleteCoverLetterAsync(CoverLetter letter, CancellationToken cancellationToken = default);

    // Portfolio Versions
    Task<IReadOnlyList<PortfolioVersion>> GetAllPortfolioVersionsAsync(CancellationToken cancellationToken = default);
    Task<PortfolioVersion?> GetPortfolioVersionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PortfolioVersion> AddPortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default);
    Task UpdatePortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default);
    Task DeletePortfolioVersionAsync(PortfolioVersion portfolio, CancellationToken cancellationToken = default);

    // Submissions
    Task<IReadOnlyList<ResumeSubmission>> GetAllSubmissionsAsync(Guid? profileId = null, Guid? versionId = null, CancellationToken cancellationToken = default);
    Task<ResumeSubmission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResumeSubmission> AddSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default);
    Task UpdateSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default);
    Task DeleteSubmissionAsync(ResumeSubmission submission, CancellationToken cancellationToken = default);

    // Snapshots
    Task<SubmissionSnapshot?> GetSnapshotBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<SubmissionSnapshot> AddSnapshotAsync(SubmissionSnapshot snapshot, CancellationToken cancellationToken = default);
}

