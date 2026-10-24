using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Application.Features.Tags.DTOs;
using CareerOS.Application.Features.Documents.DTOs;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;

namespace CareerOS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Job, JobDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
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
    }
}
