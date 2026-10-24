using AutoMapper;
using CareerOS.Application.Features.Profile.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Profile.Commands.SocialLinks;

public record AddSocialLinkCommand(
    SocialPlatform Platform,
    string Url,
    string? DisplayLabel = null,
    int DisplayOrder = 0) : IRequest<SocialLinkDto>;

public class AddSocialLinkCommandValidator : AbstractValidator<AddSocialLinkCommand>
{
    public AddSocialLinkCommandValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var outUri) &&
                         (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("URL must be a valid HTTP or HTTPS address.");

        RuleFor(x => x.DisplayLabel).MaximumLength(100);
    }
}

public sealed class AddSocialLinkCommandHandler : IRequestHandler<AddSocialLinkCommand, SocialLinkDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddSocialLinkCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SocialLinkDto> Handle(AddSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.Profiles.GetProfileAsync(true, cancellationToken)
            ?? throw new NotFoundException("UserProfile", "Default");

        var socialLink = SocialLink.Create(
            profile.Id,
            request.Platform,
            request.Url,
            request.DisplayLabel,
            request.DisplayOrder);

        await _uow.Profiles.AddSocialLinkAsync(socialLink, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SocialLinkDto>(socialLink);
    }
}

public record UpdateSocialLinkCommand(
    Guid Id,
    SocialPlatform Platform,
    string Url,
    string? DisplayLabel = null,
    int DisplayOrder = 0) : IRequest<SocialLinkDto>;

public class UpdateSocialLinkCommandValidator : AbstractValidator<UpdateSocialLinkCommand>
{
    public UpdateSocialLinkCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var outUri) &&
                         (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("URL must be a valid HTTP or HTTPS address.");

        RuleFor(x => x.DisplayLabel).MaximumLength(100);
    }
}

public sealed class UpdateSocialLinkCommandHandler : IRequestHandler<UpdateSocialLinkCommand, SocialLinkDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateSocialLinkCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SocialLinkDto> Handle(UpdateSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var link = await _uow.Profiles.GetSocialLinkByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("SocialLink", request.Id);

        link.Platform = request.Platform;
        link.Url = request.Url.Trim();
        link.DisplayLabel = request.DisplayLabel?.Trim();
        link.DisplayOrder = request.DisplayOrder;
        link.UpdatedAt = DateTime.UtcNow;

        await _uow.Profiles.UpdateSocialLinkAsync(link, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SocialLinkDto>(link);
    }
}

public record DeleteSocialLinkCommand(Guid Id) : IRequest;

public sealed class DeleteSocialLinkCommandHandler : IRequestHandler<DeleteSocialLinkCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteSocialLinkCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteSocialLinkCommand request, CancellationToken cancellationToken)
    {
        await _uow.Profiles.DeleteSocialLinkAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

