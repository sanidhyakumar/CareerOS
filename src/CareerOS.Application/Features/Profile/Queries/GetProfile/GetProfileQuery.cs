using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Profile.Queries.GetProfile;

public record GetProfileQuery : IRequest<UserProfileDto>;

public sealed class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken);
        if (profile == null)
        {
            // Seed a default profile if none exists yet
            profile = UserProfile.Create(
                firstName: "User",
                lastName: "Name",
                email: "user@careeros.local",
                headline: "Software Professional",
                phoneNumber: "+1 (555) 000-0000");

            await _uow.Profiles.AddAsync(profile, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<UserProfileDto>(profile);
    }
}

