using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Profile.Commands.Citizenships;

public record AddCitizenshipCommand(
    Guid CountryId,
    bool IsPrimary = false,
    DateTime? AcquisitionDate = null,
    string? PassportNumber = null,
    string? Notes = null) : IRequest<ProfileCitizenshipDto>;

public class AddCitizenshipCommandValidator : AbstractValidator<AddCitizenshipCommand>
{
    public AddCitizenshipCommandValidator()
    {
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country is required.");
        RuleFor(x => x.PassportNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public sealed class AddCitizenshipCommandHandler : IRequestHandler<AddCitizenshipCommand, ProfileCitizenshipDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddCitizenshipCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ProfileCitizenshipDto> Handle(AddCitizenshipCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        // If this is set to primary, reset other citizenships
        if (request.IsPrimary)
        {
            foreach (var existing in profile.Citizenships)
            {
                existing.IsPrimary = false;
            }
        }
        else if (!profile.Citizenships.Any())
        {
            // First citizenship is primary by default
            request = request with { IsPrimary = true };
        }

        var citizenship = ProfileCitizenship.Create(
            profile.Id,
            country.Id,
            request.IsPrimary,
            request.AcquisitionDate,
            request.PassportNumber,
            request.Notes);

        await _uow.Profiles.AddCitizenshipAsync(citizenship, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        citizenship.Country = country;
        return _mapper.Map<ProfileCitizenshipDto>(citizenship);
    }
}

public record DeleteCitizenshipCommand(Guid Id) : IRequest;

public sealed class DeleteCitizenshipCommandHandler : IRequestHandler<DeleteCitizenshipCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteCitizenshipCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteCitizenshipCommand request, CancellationToken cancellationToken)
    {
        await _uow.Profiles.DeleteCitizenshipAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public record SetPrimaryCitizenshipCommand(Guid Id) : IRequest<ProfileCitizenshipDto>;

public sealed class SetPrimaryCitizenshipCommandHandler : IRequestHandler<SetPrimaryCitizenshipCommand, ProfileCitizenshipDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SetPrimaryCitizenshipCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ProfileCitizenshipDto> Handle(SetPrimaryCitizenshipCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var target = profile.Citizenships.FirstOrDefault(c => c.Id == request.Id)
            ?? throw new NotFoundException("ProfileCitizenship", request.Id);

        foreach (var c in profile.Citizenships)
        {
            c.IsPrimary = (c.Id == request.Id);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ProfileCitizenshipDto>(target);
    }
}

