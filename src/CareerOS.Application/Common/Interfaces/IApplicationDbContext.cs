using CareerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerOS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Job> Jobs { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<ContactInteraction> ContactInteractions { get; }
    DbSet<Resume> Resumes { get; }
    DbSet<Country> Countries { get; }
    DbSet<Region> Regions { get; }
    DbSet<Document> Documents { get; }
    DbSet<Tag> Tags { get; }
    DbSet<DocumentTag> DocumentTags { get; }
    DbSet<DocumentUsage> DocumentUsages { get; }
    DbSet<UserProfile> Profiles { get; }
    DbSet<ProfileCitizenship> ProfileCitizenships { get; }
    DbSet<WorkAuthorization> WorkAuthorizations { get; }
    DbSet<SocialLink> SocialLinks { get; }
    DbSet<Address> Addresses { get; }
    DbSet<ResumeProfile> ResumeProfiles { get; }
    DbSet<ResumeVersion> ResumeVersions { get; }
    DbSet<CoverLetter> CoverLetters { get; }
    DbSet<PortfolioVersion> PortfolioVersions { get; }
    DbSet<ResumeSubmission> ResumeSubmissions { get; }
    DbSet<SubmissionSnapshot> SubmissionSnapshots { get; }
    DbSet<Company> Companies { get; }
    DbSet<JobApplication> JobApplications { get; }
    DbSet<ApplicationTimelineEvent> ApplicationTimelineEvents { get; }
    DbSet<ApplicationSnapshot> ApplicationSnapshots { get; }
    DbSet<Interview> Interviews { get; }
    DbSet<InterviewInterviewer> InterviewInterviewers { get; }
    DbSet<WikiArticle> WikiArticles { get; }
    DbSet<WikiArticleLink> WikiArticleLinks { get; }
    DbSet<SponsorshipCase> SponsorshipCases { get; }
    DbSet<SponsorshipEvidence> SponsorshipEvidences { get; }
    DbSet<KanbanCard> KanbanCards { get; }
    DbSet<ReminderItem> ReminderItems { get; }
    DbSet<CredentialItem> Credentials { get; }
    DbSet<SecuritySetting> SecuritySettings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
