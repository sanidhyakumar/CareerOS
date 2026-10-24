using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Commands.CoverLetters;

public record CreateCoverLetterCommand(
    string Title,
    string Body,
    Guid? ResumeProfileId = null,
    string? TargetCompany = null,
    string? TargetRole = null,
    string? RecipientName = null,
    string? RecipientTitle = null,
    bool IsTemplate = false) : IRequest<CoverLetterDto>;

public class CreateCoverLetterCommandValidator : AbstractValidator<CreateCoverLetterCommand>
{
    public CreateCoverLetterCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty();
    }
}

public class CreateCoverLetterCommandHandler : IRequestHandler<CreateCoverLetterCommand, CoverLetterDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCoverLetterCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CoverLetterDto> Handle(CreateCoverLetterCommand request, CancellationToken cancellationToken)
    {
        if (request.ResumeProfileId.HasValue)
        {
            var profile = await _uow.ResumeCenter.GetProfileByIdAsync(request.ResumeProfileId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(ResumeProfile), request.ResumeProfileId.Value);
        }

        var letter = CoverLetter.Create(
            request.Title,
            request.Body,
            request.ResumeProfileId,
            request.TargetCompany,
            request.TargetRole,
            request.RecipientName,
            request.RecipientTitle,
            request.IsTemplate);

        await _uow.ResumeCenter.AddCoverLetterAsync(letter, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetCoverLetterByIdAsync(letter.Id, cancellationToken);
        return _mapper.Map<CoverLetterDto>(reloaded ?? letter);
    }
}

public record UpdateCoverLetterCommand(
    Guid Id,
    string Title,
    string Body,
    Guid? ResumeProfileId,
    string? TargetCompany,
    string? TargetRole,
    string? RecipientName,
    string? RecipientTitle,
    bool IsTemplate) : IRequest<CoverLetterDto>;

public class UpdateCoverLetterCommandValidator : AbstractValidator<UpdateCoverLetterCommand>
{
    public UpdateCoverLetterCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty();
    }
}

public class UpdateCoverLetterCommandHandler : IRequestHandler<UpdateCoverLetterCommand, CoverLetterDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateCoverLetterCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CoverLetterDto> Handle(UpdateCoverLetterCommand request, CancellationToken cancellationToken)
    {
        var letter = await _uow.ResumeCenter.GetCoverLetterByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CoverLetter), request.Id);

        letter.Update(
            request.Title,
            request.Body,
            request.ResumeProfileId,
            request.TargetCompany,
            request.TargetRole,
            request.RecipientName,
            request.RecipientTitle,
            request.IsTemplate);

        await _uow.ResumeCenter.UpdateCoverLetterAsync(letter, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var reloaded = await _uow.ResumeCenter.GetCoverLetterByIdAsync(letter.Id, cancellationToken);
        return _mapper.Map<CoverLetterDto>(reloaded ?? letter);
    }
}

public record DeleteCoverLetterCommand(Guid Id) : IRequest<Unit>;

public class DeleteCoverLetterCommandHandler : IRequestHandler<DeleteCoverLetterCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteCoverLetterCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteCoverLetterCommand request, CancellationToken cancellationToken)
    {
        var letter = await _uow.ResumeCenter.GetCoverLetterByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CoverLetter), request.Id);

        await _uow.ResumeCenter.DeleteCoverLetterAsync(letter, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

