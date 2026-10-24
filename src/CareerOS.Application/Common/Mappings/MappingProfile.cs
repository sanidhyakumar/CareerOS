using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Application.Features.Tags.DTOs;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Application.Features.Companies.DTOs;
using CareerOS.Domain.Entities;

namespace CareerOS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Job, JobDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.CompanyEntity != null ? s.CompanyEntity.Name : s.Company));
        CreateMap<Contact, ContactDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.CompanyEntity != null ? s.CompanyEntity.Name : s.Company))
            .ForMember(d => d.PositionTitle, opt => opt.MapFrom(s => s.JobEntity != null ? s.JobEntity.Title : null))
            .ForMember(d => d.InteractionsCount, opt => opt.MapFrom(s => s.Interactions != null ? s.Interactions.Count : 0))
            .ForMember(d => d.Interactions, opt => opt.MapFrom(s => s.Interactions != null ? s.Interactions.OrderByDescending(i => i.InteractionDate).ToList() : new List<ContactInteraction>()));
        CreateMap<ContactInteraction, ContactInteractionDto>();
        CreateMap<Resume, ResumeDto>().ReverseMap();

        CreateMap<Country, CountryDto>()
            .ForMember(d => d.RegionCount, opt => opt.MapFrom(s => s.Regions.Count));
        CreateMap<Region, RegionDto>();
        CreateMap<Tag, TagDto>();

        CreateMap<Document, DocumentDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : null))
            .ForMember(d => d.RegionName, opt => opt.MapFrom(s => s.Region != null ? s.Region.Name : null))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.DocumentTags.Select(dt => dt.Tag)))
            .ForMember(d => d.Usages, opt => opt.MapFrom(s => s.Usages));

        CreateMap<Document, DocumentVersionDto>();
        CreateMap<DocumentUsage, DocumentUsageDto>();

        // Phase 2: Personal Profile mappings
        CreateMap<UserProfile, UserProfileDto>();

        CreateMap<ProfileCitizenship, ProfileCitizenshipDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty))
            .ForMember(d => d.CountryCode, opt => opt.MapFrom(s => s.Country != null ? s.Country.Code : string.Empty));

        CreateMap<WorkAuthorization, WorkAuthorizationDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty))
            .ForMember(d => d.CountryCode, opt => opt.MapFrom(s => s.Country != null ? s.Country.Code : string.Empty))
            .ForMember(d => d.DocumentName, opt => opt.MapFrom(s => s.Document != null ? s.Document.OriginalFileName : null));

        CreateMap<SocialLink, SocialLinkDto>();

        CreateMap<Address, AddressDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty))
            .ForMember(d => d.CountryCode, opt => opt.MapFrom(s => s.Country != null ? s.Country.Code : string.Empty))
            .ForMember(d => d.RegionName, opt => opt.MapFrom(s => s.Region != null ? s.Region.Name : null))
            .ForMember(d => d.RegionCode, opt => opt.MapFrom(s => s.Region != null ? s.Region.Code : null));

        // Phase 4: Resume Center mappings
        CreateMap<ResumeProfile, ResumeProfileDto>()
            .ForMember(d => d.VersionCount, opt => opt.MapFrom(s => s.Versions.Count))
            .ForMember(d => d.SubmissionCount, opt => opt.MapFrom(s => s.Submissions.Count))
            .ForMember(d => d.Versions, opt => opt.MapFrom(s => s.Versions));

        CreateMap<ResumeVersion, ResumeVersionDto>()
            .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.ResumeProfile != null ? s.ResumeProfile.Name : null))
            .ForMember(d => d.DocumentFileName, opt => opt.MapFrom(s => s.Document != null ? s.Document.OriginalFileName : null))
            .ForMember(d => d.DocumentSha256, opt => opt.MapFrom(s => s.Document != null ? s.Document.Sha256Hash : null))
            .ForMember(d => d.SubmissionCount, opt => opt.MapFrom(s => s.Submissions.Count));

        CreateMap<CoverLetter, CoverLetterDto>()
            .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.ResumeProfile != null ? s.ResumeProfile.Name : null));

        CreateMap<PortfolioVersion, PortfolioVersionDto>()
            .ForMember(d => d.SubmissionCount, opt => opt.MapFrom(s => s.Submissions.Count));

        CreateMap<ResumeSubmission, ResumeSubmissionDto>()
            .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.ResumeProfile != null ? s.ResumeProfile.Name : null))
            .ForMember(d => d.VersionName, opt => opt.MapFrom(s => s.ResumeVersion != null ? s.ResumeVersion.VersionName : null))
            .ForMember(d => d.VersionNumber, opt => opt.MapFrom(s => s.ResumeVersion != null ? s.ResumeVersion.VersionNumber : 1))
            .ForMember(d => d.CoverLetterTitle, opt => opt.MapFrom(s => s.CoverLetter != null ? s.CoverLetter.Title : null))
            .ForMember(d => d.PortfolioName, opt => opt.MapFrom(s => s.PortfolioVersion != null ? s.PortfolioVersion.Name : null));

        CreateMap<SubmissionSnapshot, SubmissionSnapshotDto>();

        // Phase 5: Company CRM mappings
        CreateMap<Company, CompanyDto>()
            .ForMember(d => d.JobsCount, opt => opt.MapFrom(s => s.Jobs.Count))
            .ForMember(d => d.ContactsCount, opt => opt.MapFrom(s => s.Contacts.Count));

        // Phase 7: Application Tracking mappings
        CreateMap<JobApplication, CareerOS.Application.Features.Applications.DTOs.JobApplicationDto>()
            .ForMember(d => d.JobTitle, opt => opt.MapFrom(s => s.Job != null ? s.Job.Title : string.Empty))
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Job != null && !string.IsNullOrEmpty(s.Job.Company) ? s.Job.Company : (s.Company != null ? s.Company.Name : string.Empty)))
            .ForMember(d => d.JobDepartment, opt => opt.MapFrom(s => s.Job != null ? s.Job.Department : null))
            .ForMember(d => d.JobLocation, opt => opt.MapFrom(s => s.Job != null ? s.Job.Location : null))
            .ForMember(d => d.JobSalary, opt => opt.MapFrom(s => s.Job != null ? s.Job.Salary : null))
            .ForMember(d => d.TechStack, opt => opt.MapFrom(s => s.Job != null ? s.Job.TechStack : null))
            .ForMember(d => d.ResumeVersionName, opt => opt.MapFrom(s => s.ResumeVersion != null ? s.ResumeVersion.VersionName : null))
            .ForMember(d => d.ResumeProfileName, opt => opt.MapFrom(s => s.ResumeVersion != null && s.ResumeVersion.ResumeProfile != null ? s.ResumeVersion.ResumeProfile.Name : null))
            .ForMember(d => d.CoverLetterTitle, opt => opt.MapFrom(s => s.CoverLetter != null ? s.CoverLetter.Title : null))
            .ForMember(d => d.PortfolioName, opt => opt.MapFrom(s => s.PortfolioVersion != null ? s.PortfolioVersion.Name : null))
            .ForMember(d => d.ReferralContactName, opt => opt.MapFrom(s => s.ReferralContact != null ? s.ReferralContact.Name : null))
            .ForMember(d => d.TimelineEventsCount, opt => opt.MapFrom(s => s.TimelineEvents != null ? s.TimelineEvents.Count : 0))
            .ForMember(d => d.LatestEventTitle, opt => opt.MapFrom(s => s.TimelineEvents != null && s.TimelineEvents.Any() ? s.TimelineEvents.OrderByDescending(e => e.EventDate).First().Title : null))
            .ForMember(d => d.LatestEventDate, opt => opt.MapFrom(s => s.TimelineEvents != null && s.TimelineEvents.Any() ? s.TimelineEvents.OrderByDescending(e => e.EventDate).First().EventDate : (DateTime?)null))
            .ForMember(d => d.TimelineEvents, opt => opt.MapFrom(s => s.TimelineEvents != null ? s.TimelineEvents.OrderBy(e => e.EventDate).ToList() : new List<ApplicationTimelineEvent>()));

        CreateMap<ApplicationTimelineEvent, CareerOS.Application.Features.Applications.DTOs.ApplicationTimelineEventDto>();
        CreateMap<ApplicationSnapshot, CareerOS.Application.Features.Applications.DTOs.ApplicationSnapshotDto>();

        // Phase 9: Interview Center mappings
        CreateMap<Interview, CareerOS.Application.Features.Interviews.DTOs.InterviewDto>()
            .ForMember(d => d.JobTitle, opt => opt.MapFrom(s => s.Job != null ? s.Job.Title : (s.JobApplication != null && s.JobApplication.Job != null ? s.JobApplication.Job.Title : null)))
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company != null ? s.Company.Name : (s.Job != null ? s.Job.Company : (s.JobApplication != null && s.JobApplication.Job != null ? s.JobApplication.Job.Company : null))))
            .ForMember(d => d.Interviewers, opt => opt.MapFrom(s => s.Interviewers));

        CreateMap<InterviewInterviewer, CareerOS.Application.Features.Interviews.DTOs.InterviewInterviewerDto>()
            .ForMember(d => d.ContactName, opt => opt.MapFrom(s => s.Contact != null ? s.Contact.Name : string.Empty))
            .ForMember(d => d.ContactEmail, opt => opt.MapFrom(s => s.Contact != null ? s.Contact.Email : null))
            .ForMember(d => d.ContactLinkedIn, opt => opt.MapFrom(s => s.Contact != null ? s.Contact.LinkedIn : null));

        // Wiki Mappings
        CreateMap<CareerOS.Domain.Entities.WikiArticle, CareerOS.Application.Features.Wiki.DTOs.WikiArticleDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company != null ? s.Company.Name : null))
            .ForMember(d => d.JobTitle, opt => opt.MapFrom(s => s.Job != null ? s.Job.Title : null))
            .ForMember(d => d.Backlinks, opt => opt.MapFrom(s => s.Inlinks.Select(l => new CareerOS.Application.Features.Wiki.DTOs.WikiBacklinkDto
            {
                ArticleId = l.SourceArticleId,
                Title = l.SourceArticle != null ? l.SourceArticle.Title : string.Empty,
                Slug = l.SourceArticle != null ? l.SourceArticle.Slug : string.Empty,
                CategoryName = l.SourceArticle != null ? l.SourceArticle.Category.ToString() : string.Empty
            })))
            .ForMember(d => d.Outlinks, opt => opt.MapFrom(s => s.Outlinks.Select(l => new CareerOS.Application.Features.Wiki.DTOs.WikiOutlinkDto
            {
                TargetSlug = l.TargetSlug,
                TargetTitle = l.TargetArticle != null ? l.TargetArticle.Title : null,
                TargetArticleId = l.TargetArticleId,
                LinkText = l.LinkText
            })));

        // Sponsorship Mappings
        CreateMap<CareerOS.Domain.Entities.SponsorshipCase, CareerOS.Application.Features.Sponsorship.DTOs.SponsorshipCaseDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty))
            .ForMember(d => d.CountryCode, opt => opt.MapFrom(s => s.Country != null ? s.Country.Code : string.Empty))
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company != null ? s.Company.Name : null))
            .ForMember(d => d.JobTitle, opt => opt.MapFrom(s => s.Job != null ? s.Job.Title : null))
            .ForMember(d => d.EvidenceList, opt => opt.MapFrom(s => s.EvidenceList));

        CreateMap<CareerOS.Domain.Entities.SponsorshipEvidence, CareerOS.Application.Features.Sponsorship.DTOs.SponsorshipEvidenceDto>()
            .ForMember(d => d.DocumentName, opt => opt.MapFrom(s => s.Document != null ? s.Document.FileName : null))
            .ForMember(d => d.DocumentType, opt => opt.MapFrom(s => s.Document != null ? s.Document.Category.ToString() : null));

        // Kanban Mappings
        CreateMap<CareerOS.Domain.Entities.KanbanCard, CareerOS.Application.Features.Kanban.DTOs.KanbanCardDto>();
    }
}
