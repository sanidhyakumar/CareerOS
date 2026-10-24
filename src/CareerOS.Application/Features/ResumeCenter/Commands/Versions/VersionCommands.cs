using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Commands.Versions;

public record CreateResumeVersionCommand(
    Guid ResumeProfileId,
    string VersionName,
    Guid? DocumentId = null,
    string FileFormat = "PDF",
    string? TargetCompany = null,
    string? TargetJobTitle = null,
    string? TailoredKeywords = null,
    string? Content = null,
    string? ChangeNotes = null,
    bool IsBaseVersion = false) : IRequest<ResumeVersionDto>;

public class CreateResumeVersionCommandValidator : AbstractValidator<CreateResumeVersionCommand>
{
    public CreateResumeVersionCommandValidator()
    {
        RuleFor(x => x.ResumeProfileId).NotEmpty();
        RuleFor(x => x.VersionName).NotEmpty().MaximumLength(200);
    }
}

public class CreateResumeVersionCommandHandler : IRequestHandler<CreateResumeVersionCommand, ResumeVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateResumeVersionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeVersionDto> Handle(CreateResumeVersionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.ResumeProfileId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeProfile), request.ResumeProfileId);

        int nextVersionNumber = await _uow.ResumeCenter.GetNextVersionNumberAsync(request.ResumeProfileId, cancellationToken);

        var version = ResumeVersion.Create(
            request.ResumeProfileId,
            request.VersionName,
            nextVersionNumber,
            request.DocumentId,
            request.FileFormat,
            request.TargetCompany,
            request.TargetJobTitle,
            request.TailoredKeywords,
            request.Content,
            request.ChangeNotes,
            request.IsBaseVersion);

        await _uow.ResumeCenter.AddVersionAsync(version, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        // Fetch reloaded version with details
        var reloaded = await _uow.ResumeCenter.GetVersionByIdAsync(version.Id, cancellationToken);
        return _mapper.Map<ResumeVersionDto>(reloaded ?? version);
    }
}

public record UpdateResumeVersionCommand(
    Guid Id,
    string VersionName,
    Guid? DocumentId,
    string FileFormat,
    string? TargetCompany,
    string? TargetJobTitle,
    string? TailoredKeywords,
    string? Content,
    string? ChangeNotes,
    bool IsBaseVersion,
    bool IsArchived) : IRequest<ResumeVersionDto>;

public class UpdateResumeVersionCommandValidator : AbstractValidator<UpdateResumeVersionCommand>
{
    public UpdateResumeVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.VersionName).NotEmpty().MaximumLength(200);
    }
}

public class UpdateResumeVersionCommandHandler : IRequestHandler<UpdateResumeVersionCommand, ResumeVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateResumeVersionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeVersionDto> Handle(UpdateResumeVersionCommand request, CancellationToken cancellationToken)
    {
        var version = await _uow.ResumeCenter.GetVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeVersion), request.Id);

        version.Update(
            request.VersionName,
            request.DocumentId,
            request.FileFormat,
            request.TargetCompany,
            request.TargetJobTitle,
            request.TailoredKeywords,
            request.Content,
            request.ChangeNotes,
            request.IsBaseVersion,
            request.IsArchived);

        await _uow.ResumeCenter.UpdateVersionAsync(version, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetVersionByIdAsync(version.Id, cancellationToken);
        return _mapper.Map<ResumeVersionDto>(reloaded ?? version);
    }
}

public record DeleteResumeVersionCommand(Guid Id) : IRequest<Unit>;

public class DeleteResumeVersionCommandHandler : IRequestHandler<DeleteResumeVersionCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteResumeVersionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteResumeVersionCommand request, CancellationToken cancellationToken)
    {
        var version = await _uow.ResumeCenter.GetVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeVersion), request.Id);

        await _uow.ResumeCenter.DeleteVersionAsync(version, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public record CloneResumeVersionCommand(Guid Id, string? NewVersionName = null, string? TargetCompany = null) : IRequest<ResumeVersionDto>;

public class CloneResumeVersionCommandHandler : IRequestHandler<CloneResumeVersionCommand, ResumeVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CloneResumeVersionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeVersionDto> Handle(CloneResumeVersionCommand request, CancellationToken cancellationToken)
    {
        var source = await _uow.ResumeCenter.GetVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeVersion), request.Id);

        int nextVersionNumber = await _uow.ResumeCenter.GetNextVersionNumberAsync(source.ResumeProfileId, cancellationToken);

        string newName = !string.IsNullOrWhiteSpace(request.NewVersionName)
            ? request.NewVersionName
            : $"{source.VersionName} (Copy v{nextVersionNumber})";

        var cloned = ResumeVersion.Create(
            source.ResumeProfileId,
            newName,
            nextVersionNumber,
            source.DocumentId,
            source.FileFormat,
            request.TargetCompany ?? source.TargetCompany,
            source.TargetJobTitle,
            source.TailoredKeywords,
            source.Content,
            $"Cloned from version {source.VersionNumber} ({source.VersionName})",
            isBaseVersion: false);

        await _uow.ResumeCenter.AddVersionAsync(cloned, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetVersionByIdAsync(cloned.Id, cancellationToken);
        return _mapper.Map<ResumeVersionDto>(reloaded ?? cloned);
    }
}

