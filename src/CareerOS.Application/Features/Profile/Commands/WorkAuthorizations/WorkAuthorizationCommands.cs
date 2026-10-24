using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Profile.Commands.WorkAuthorizations;

public record AddWorkAuthorizationCommand(
    Guid CountryId,
    WorkAuthorizationType AuthorizationType,
    string? VisaType = null,
    bool RequiresSponsorship = false,
    DateTime? ValidFrom = null,
    DateTime? ValidTo = null,
    Guid? DocumentId = null,
    string? Notes = null) : IRequest<WorkAuthorizationDto>;

public class AddWorkAuthorizationCommandValidator : AbstractValidator<AddWorkAuthorizationCommand>
{
    public AddWorkAuthorizationCommandValidator()
    {
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country is required.");
        RuleFor(x => x.VisaType).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x)
            .Must(x => !x.ValidFrom.HasValue || !x.ValidTo.HasValue || x.ValidTo >= x.ValidFrom)
            .WithMessage("ValidTo date cannot be earlier than ValidFrom date.");
    }
}

public sealed class AddWorkAuthorizationCommandHandler : IRequestHandler<AddWorkAuthorizationCommand, WorkAuthorizationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddWorkAuthorizationCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WorkAuthorizationDto> Handle(AddWorkAuthorizationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        Document? doc = null;
        if (request.DocumentId.HasValue)
        {
            doc = await _uow.Documents.GetByIdAsync(request.DocumentId.Value, false, cancellationToken);
        }

        var workAuth = WorkAuthorization.Create(
            profile.Id,
            country.Id,
            request.AuthorizationType,
            request.VisaType,
            request.RequiresSponsorship,
            request.ValidFrom,
            request.ValidTo,
            request.DocumentId,
            request.Notes);

        await _uow.Profiles.AddWorkAuthorizationAsync(workAuth, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        workAuth.Country = country;
        workAuth.Document = doc;
        return _mapper.Map<WorkAuthorizationDto>(workAuth);
    }
}

public record UpdateWorkAuthorizationCommand(
    Guid Id,
    Guid CountryId,
    WorkAuthorizationType AuthorizationType,
    string? VisaType = null,
    bool RequiresSponsorship = false,
    DateTime? ValidFrom = null,
    DateTime? ValidTo = null,
    Guid? DocumentId = null,
    string? Notes = null) : IRequest<WorkAuthorizationDto>;

public class UpdateWorkAuthorizationCommandValidator : AbstractValidator<UpdateWorkAuthorizationCommand>
{
    public UpdateWorkAuthorizationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.VisaType).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x)
            .Must(x => !x.ValidFrom.HasValue || !x.ValidTo.HasValue || x.ValidTo >= x.ValidFrom)
            .WithMessage("ValidTo date cannot be earlier than ValidFrom date.");
    }
}

public sealed class UpdateWorkAuthorizationCommandHandler : IRequestHandler<UpdateWorkAuthorizationCommand, WorkAuthorizationDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateWorkAuthorizationCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<WorkAuthorizationDto> Handle(UpdateWorkAuthorizationCommand request, CancellationToken cancellationToken)
    {
        var workAuth = await _uow.Profiles.GetWorkAuthorizationByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("WorkAuthorization", request.Id);

        var country = await _uow.Countries.GetByIdAsync(request.CountryId, false, cancellationToken)
            ?? throw new NotFoundException("Country", request.CountryId);

        Document? doc = null;
        if (request.DocumentId.HasValue)
        {
            doc = await _uow.Documents.GetByIdAsync(request.DocumentId.Value, false, cancellationToken);
        }

        workAuth.CountryId = request.CountryId;
        workAuth.AuthorizationType = request.AuthorizationType;
        workAuth.VisaType = request.VisaType?.Trim();
        workAuth.RequiresSponsorship = request.RequiresSponsorship;
        workAuth.ValidFrom = request.ValidFrom;
        workAuth.ValidTo = request.ValidTo;
        workAuth.DocumentId = request.DocumentId;
        workAuth.Notes = request.Notes?.Trim();
        workAuth.UpdatedAt = DateTime.UtcNow;

        await _uow.Profiles.UpdateWorkAuthorizationAsync(workAuth, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        workAuth.Country = country;
        workAuth.Document = doc;
        return _mapper.Map<WorkAuthorizationDto>(workAuth);
    }
}

public record DeleteWorkAuthorizationCommand(Guid Id) : IRequest;

public sealed class DeleteWorkAuthorizationCommandHandler : IRequestHandler<DeleteWorkAuthorizationCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteWorkAuthorizationCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteWorkAuthorizationCommand request, CancellationToken cancellationToken)
    {
        await _uow.Profiles.DeleteWorkAuthorizationAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

