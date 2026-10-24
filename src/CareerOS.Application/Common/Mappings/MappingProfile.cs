using AutoMapper;
using CareerOS.Application.Features.Jobs.DTOs;
using CareerOS.Application.Features.Contacts.DTOs;
using CareerOS.Application.Features.Resumes.DTOs;
using CareerOS.Domain.Entities;

namespace CareerOS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Job, JobDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Resume, ResumeDto>().ReverseMap();
    }
}

