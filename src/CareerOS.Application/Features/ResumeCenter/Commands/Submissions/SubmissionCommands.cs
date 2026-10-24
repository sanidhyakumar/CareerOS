using System.Text.Json;
using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Commands.Submissions;

public record CreateResumeSubmissionCommand(
    Guid ResumeProfileId,
    Guid ResumeVersionId,
    string CompanyName,
    string RoleTitle,
    DateTime? SubmissionDate = null,
    string Channel = "Company Website",
    string Status = "Submitted",
    Guid? CoverLetterId = null,
    Guid? PortfolioVersionId = null,
    string? Notes = null) : IRequest<ResumeSubmissionDto>;

public class CreateResumeSubmissionCommandValidator : AbstractValidator<CreateResumeSubmissionCommand>
{
    public CreateResumeSubmissionCommandValidator()
    {
        RuleFor(x => x.ResumeProfileId).NotEmpty();
        RuleFor(x => x.ResumeVersionId).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RoleTitle).NotEmpty().MaximumLength(200);
    }
}

public class CreateResumeSubmissionCommandHandler : IRequestHandler<CreateResumeSubmissionCommand, ResumeSubmissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateResumeSubmissionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeSubmissionDto> Handle(CreateResumeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.ResumeProfileId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeProfile), request.ResumeProfileId);

        var version = await _uow.ResumeCenter.GetVersionByIdAsync(request.ResumeVersionId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeVersion), request.ResumeVersionId);

        CoverLetter? coverLetter = null;
        if (request.CoverLetterId.HasValue)
        {
            coverLetter = await _uow.ResumeCenter.GetCoverLetterByIdAsync(request.CoverLetterId.Value, cancellationToken);
        }

        PortfolioVersion? portfolio = null;
        if (request.PortfolioVersionId.HasValue)
        {
            portfolio = await _uow.ResumeCenter.GetPortfolioVersionByIdAsync(request.PortfolioVersionId.Value, cancellationToken);
        }

        Document? attachedDocument = null;
        if (version.DocumentId.HasValue)
        {
            attachedDocument = await _uow.Documents.GetByIdAsync(version.DocumentId.Value, cancellationToken: cancellationToken);
        }

        DateTime subDate = request.SubmissionDate ?? DateTime.UtcNow;

        var submission = ResumeSubmission.Create(
            request.ResumeProfileId,
            request.ResumeVersionId,
            request.CompanyName,
            request.RoleTitle,
            subDate,
            request.Channel,
            request.Status,
            request.CoverLetterId,
            request.PortfolioVersionId,
            request.Notes);

        await _uow.ResumeCenter.AddSubmissionAsync(submission, cancellationToken);

        // Snapshot System: Create Immutable submission snapshot
        var metadata = new
        {
            TailoredKeywords = version.TailoredKeywords,
            TargetJobTitle = version.TargetJobTitle,
            TargetCompany = version.TargetCompany,
            FileFormat = version.FileFormat
        };

        var snapshot = SubmissionSnapshot.Create(
            submission.Id,
            profile.Name,
            version.VersionName,
            version.VersionNumber,
            resumeDocumentSha256: attachedDocument?.Sha256Hash,
            resumeDocumentFileName: attachedDocument?.OriginalFileName,
            resumeContentSnapshot: version.Content,
            coverLetterTitle: coverLetter?.Title,
            coverLetterBodySnapshot: coverLetter?.Body,
            portfolioName: portfolio?.Name,
            portfolioUrlSnapshot: portfolio?.Url,
            metadataJson: JsonSerializer.Serialize(metadata));

        await _uow.ResumeCenter.AddSnapshotAsync(snapshot, cancellationToken);
        submission.SnapshotId = snapshot.Id;

        // Document Usage Linkage (Phase 3 Vault tracking)
        if (attachedDocument != null)
        {
            var usage = DocumentUsage.Create(
                attachedDocument.Id,
                "ResumeSubmission",
                submission.Id,
                $"{request.CompanyName} - {request.RoleTitle}",
                $"Submitted via {request.Channel}");
            await _uow.Documents.AddUsageAsync(usage, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetSubmissionByIdAsync(submission.Id, cancellationToken);
        return _mapper.Map<ResumeSubmissionDto>(reloaded ?? submission);
    }
}

public record UpdateResumeSubmissionCommand(
    Guid Id,
    string CompanyName,
    string RoleTitle,
    DateTime SubmissionDate,
    string Channel,
    string Status,
    Guid? CoverLetterId,
    Guid? PortfolioVersionId,
    string? Notes) : IRequest<ResumeSubmissionDto>;

public class UpdateResumeSubmissionCommandValidator : AbstractValidator<UpdateResumeSubmissionCommand>
{
    public UpdateResumeSubmissionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RoleTitle).NotEmpty().MaximumLength(200);
    }
}

public class UpdateResumeSubmissionCommandHandler : IRequestHandler<UpdateResumeSubmissionCommand, ResumeSubmissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateResumeSubmissionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ResumeSubmissionDto> Handle(UpdateResumeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _uow.ResumeCenter.GetSubmissionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeSubmission), request.Id);

        submission.Update(
            request.CompanyName,
            request.RoleTitle,
            request.SubmissionDate,
            request.Channel,
            request.Status,
            request.CoverLetterId,
            request.PortfolioVersionId,
            request.Notes);

        await _uow.ResumeCenter.UpdateSubmissionAsync(submission, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetSubmissionByIdAsync(submission.Id, cancellationToken);
        return _mapper.Map<ResumeSubmissionDto>(reloaded ?? submission);
    }
}

public record DeleteResumeSubmissionCommand(Guid Id) : IRequest<Unit>;

public class DeleteResumeSubmissionCommandHandler : IRequestHandler<DeleteResumeSubmissionCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteResumeSubmissionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteResumeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _uow.ResumeCenter.GetSubmissionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeSubmission), request.Id);

        await _uow.ResumeCenter.DeleteSubmissionAsync(submission, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

