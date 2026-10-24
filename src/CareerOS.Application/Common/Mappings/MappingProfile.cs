using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Application.Features.Tags.DTOs;
using CareerOS.Application.Features.Documents.DTOs;
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
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.DocumentTags.Select(dt => dt.Tag)));

        CreateMap<Document, DocumentVersionDto>();
    }
}
