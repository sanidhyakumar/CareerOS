using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Commands.Profiles;

public record CreateResumeProfileCommand(
    string Name,
    string TargetRole,
    string? TargetIndustry = null,
    string? Summary = null,
    string? PrimarySkills = null,
    string? SecondarySkills = null,
    bool IsDefault = false) : IRequest<ResumeProfileDto>;

public class CreateResumeProfileCommandValidator : AbstractValidator<CreateResumeProfileCommand>
{
    public CreateResumeProfileCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TargetRole).NotEmpty().MaximumLength(200);
    }
}

public class CreateResumeProfileCommandHandler : IRequestHandler<CreateResumeProfileCommand, ResumeProfileDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateResumeProfileCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeProfileDto> Handle(CreateResumeProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = ResumeProfile.Create(
            request.Name,
            request.TargetRole,
            request.TargetIndustry,
            request.Summary,
            request.PrimarySkills,
            request.SecondarySkills,
            request.IsDefault);

        await _uow.ResumeCenter.AddProfileAsync(profile, cancellationToken);
        if (request.IsDefault)
        {
            await _uow.ResumeCenter.SetDefaultProfileAsync(profile.Id, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ResumeProfileDto>(profile);
    }
}

public record UpdateResumeProfileCommand(
    Guid Id,
    string Name,
    string TargetRole,
    string? TargetIndustry,
    string? Summary,
    string? PrimarySkills,
    string? SecondarySkills,
    bool IsDefault,
    bool IsArchived) : IRequest<ResumeProfileDto>;

public class UpdateResumeProfileCommandValidator : AbstractValidator<UpdateResumeProfileCommand>
{
    public UpdateResumeProfileCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TargetRole).NotEmpty().MaximumLength(200);
    }
}

public class UpdateResumeProfileCommandHandler : IRequestHandler<UpdateResumeProfileCommand, ResumeProfileDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateResumeProfileCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeProfileDto> Handle(UpdateResumeProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeProfile), request.Id);

        profile.Update(
            request.Name,
            request.TargetRole,
            request.TargetIndustry,
            request.Summary,
            request.PrimarySkills,
            request.SecondarySkills,
            request.IsDefault,
            request.IsArchived);

        await _uow.ResumeCenter.UpdateProfileAsync(profile, cancellationToken);
        if (request.IsDefault)
        {
            await _uow.ResumeCenter.SetDefaultProfileAsync(profile.Id, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ResumeProfileDto>(profile);
    }
}

public record DeleteResumeProfileCommand(Guid Id) : IRequest<Unit>;

public class DeleteResumeProfileCommandHandler : IRequestHandler<DeleteResumeProfileCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteResumeProfileCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteResumeProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeProfile), request.Id);

        await _uow.ResumeCenter.DeleteProfileAsync(profile, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public record SetDefaultResumeProfileCommand(Guid Id) : IRequest<Unit>;

public class SetDefaultResumeProfileCommandHandler : IRequestHandler<SetDefaultResumeProfileCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public SetDefaultResumeProfileCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(SetDefaultResumeProfileCommand request, CancellationToken cancellationToken)
    {
        await _uow.ResumeCenter.SetDefaultProfileAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

