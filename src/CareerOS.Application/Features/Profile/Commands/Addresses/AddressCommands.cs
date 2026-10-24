using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Profile.Commands.Addresses;

public record AddAddressCommand(
    AddressType AddressType,
    string Street1,
    string? Street2,
    string City,
    string StateProvince,
    Guid? RegionId,
    string PostalCode,
    Guid CountryId,
    bool IsPrimary = false,
    bool WillingToRelocate = false,
    bool OpenToRemote = true) : IRequest<AddressDto>;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.Street1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Street2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StateProvince).MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(30);
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country is required.");
    }
}

public sealed class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, AddressDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddAddressCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AddressDto> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        Region? region = null;
        if (request.RegionId.HasValue)
        {
            region = await _uow.Regions.GetByIdAsync(request.RegionId.Value, cancellationToken);
        }

        if (request.IsPrimary)
        {
            foreach (var addr in profile.Addresses)
            {
                addr.IsPrimary = false;
            }
        }
        else if (!profile.Addresses.Any())
        {
            request = request with { IsPrimary = true };
        }

        var address = Address.Create(
            profile.Id,
            request.AddressType,
            request.Street1,
            request.Street2,
            request.City,
            request.StateProvince,
            request.RegionId,
            request.PostalCode,
            request.CountryId,
            request.IsPrimary,
            request.WillingToRelocate,
            request.OpenToRemote);

        await _uow.Profiles.AddAddressAsync(address, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        address.Country = country;
        address.Region = region;
        return _mapper.Map<AddressDto>(address);
    }
}

public record UpdateAddressCommand(
    Guid Id,
    AddressType AddressType,
    string Street1,
    string? Street2,
    string City,
    string StateProvince,
    Guid? RegionId,
    string PostalCode,
    Guid CountryId,
    bool IsPrimary,
    bool WillingToRelocate,
    bool OpenToRemote) : IRequest<AddressDto>;

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Street1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Street2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StateProvince).MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(30);
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country is required.");
    }
}

public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateAddressCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AddressDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _uow.Profiles.GetAddressByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Address", request.Id);

        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        Region? region = null;
        if (request.RegionId.HasValue)
        {
            region = await _uow.Regions.GetByIdAsync(request.RegionId.Value, cancellationToken);
        }

        if (request.IsPrimary)
        {
            var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken);
            if (profile != null)
            {
                foreach (var a in profile.Addresses.Where(a => a.Id != request.Id))
                {
                    a.IsPrimary = false;
                }
            }
        }

        address.AddressType = request.AddressType;
        address.Street1 = request.Street1.Trim();
        address.Street2 = request.Street2?.Trim();
        address.City = request.City.Trim();
        address.StateProvince = request.StateProvince.Trim();
        address.RegionId = request.RegionId;
        address.PostalCode = request.PostalCode.Trim();
        address.CountryId = request.CountryId;
        address.IsPrimary = request.IsPrimary;
        address.WillingToRelocate = request.WillingToRelocate;
        address.OpenToRemote = request.OpenToRemote;
        address.UpdatedAt = DateTime.UtcNow;

        await _uow.Profiles.UpdateAddressAsync(address, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        address.Country = country;
        address.Region = region;
        return _mapper.Map<AddressDto>(address);
    }
}

public record DeleteAddressCommand(Guid Id) : IRequest;

public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteAddressCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        await _uow.Profiles.DeleteAddressAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public record SetPrimaryAddressCommand(Guid Id) : IRequest<AddressDto>;

public sealed class SetPrimaryAddressCommandHandler : IRequestHandler<SetPrimaryAddressCommand, AddressDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SetPrimaryAddressCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AddressDto> Handle(SetPrimaryAddressCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var target = profile.Addresses.FirstOrDefault(a => a.Id == request.Id)
            ?? throw new NotFoundException("Address", request.Id);

        foreach (var a in profile.Addresses)
        {
            a.IsPrimary = (a.Id == request.Id);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AddressDto>(target);
    }
}

