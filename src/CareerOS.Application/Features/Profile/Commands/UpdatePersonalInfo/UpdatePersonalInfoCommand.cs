using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Profile.Commands.UpdatePersonalInfo;

public record UpdatePersonalInfoCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PreferredName,
    string? Headline,
    string Email,
    string? SecondaryEmail,
    string? PhoneNumber,
    string? SecondaryPhone,
    string? Bio,
    DateTime? DateOfBirth,
    string? AvatarUrl) : IRequest<UserProfileDto>;

public class UpdatePersonalInfoCommandValidator : AbstractValidator<UpdatePersonalInfoCommand>
{
    public UpdatePersonalInfoCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Primary email is required.")
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(255);

        RuleFor(x => x.SecondaryEmail)
            .EmailAddress().WithMessage("Valid secondary email is required.")
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.SecondaryEmail));

        RuleFor(x => x.Headline).MaximumLength(250);
        RuleFor(x => x.PhoneNumber).MaximumLength(50);
        RuleFor(x => x.SecondaryPhone).MaximumLength(50);
        RuleFor(x => x.Bio).MaximumLength(4000);
        RuleFor(x => x.AvatarUrl).MaximumLength(1000);
    }
}

public sealed class UpdatePersonalInfoCommandHandler : IRequestHandler<UpdatePersonalInfoCommand, UserProfileDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdatePersonalInfoCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(UpdatePersonalInfoCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken);
        if (profile == null)
        {
            profile = UserProfile.Create(request.FirstName, request.LastName, request.Email, request.Headline, request.PhoneNumber);
            await _uow.Profiles.AddAsync(profile, cancellationToken);
        }

        profile.UpdatePersonalInfo(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PreferredName,
            request.Headline,
            request.Email,
            request.SecondaryEmail,
            request.PhoneNumber,
            request.SecondaryPhone,
            request.Bio,
            request.DateOfBirth,
            request.AvatarUrl);

        await _uow.Profiles.UpdateAsync(profile, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var refreshed = await _uow.Profiles.GetProfileAsync(true, cancellationToken);
        return _mapper.Map<UserProfileDto>(refreshed ?? profile);
    }
}

