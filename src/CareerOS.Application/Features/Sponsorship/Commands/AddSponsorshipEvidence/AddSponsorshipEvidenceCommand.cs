using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Commands.AddSponsorshipEvidence;

public record AddSponsorshipEvidenceCommand : IRequest<SponsorshipEvidenceDto>
{
    public Guid SponsorshipCaseId { get; init; }
    public string EvidenceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? DocumentId { get; init; }
    public bool IsVerified { get; init; }
    public DateTime? ExpiryDate { get; init; }
}

public class AddSponsorshipEvidenceCommandHandler : IRequestHandler<AddSponsorshipEvidenceCommand, SponsorshipEvidenceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddSponsorshipEvidenceCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SponsorshipEvidenceDto> Handle(AddSponsorshipEvidenceCommand request, CancellationToken cancellationToken)
    {
        var sponsorship = await _uow.Sponsorship.GetByIdAsync(request.SponsorshipCaseId, includeDetails: false, cancellationToken: cancellationToken);
        if (sponsorship == null)
        {
            throw new NotFoundException(nameof(SponsorshipCase), request.SponsorshipCaseId);
        }

        var evidence = SponsorshipEvidence.Create(
            sponsorshipCaseId: request.SponsorshipCaseId,
            evidenceType: request.EvidenceType,
            description: request.Description,
            documentId: request.DocumentId,
            isVerified: request.IsVerified,
            expiryDate: request.ExpiryDate);

        await _uow.Sponsorship.AddEvidenceAsync(evidence, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var loaded = await _uow.Sponsorship.GetEvidenceByIdAsync(evidence.Id, cancellationToken);
        return _mapper.Map<SponsorshipEvidenceDto>(loaded ?? evidence);
    }
}

public class AddSponsorshipEvidenceCommandValidator : AbstractValidator<AddSponsorshipEvidenceCommand>
{
    public AddSponsorshipEvidenceCommandValidator()
    {
        RuleFor(x => x.SponsorshipCaseId)
            .NotEmpty().WithMessage("Sponsorship Case ID is required.");

        RuleFor(x => x.EvidenceType)
            .NotEmpty().WithMessage("Evidence type is required.")
            .MaximumLength(150).WithMessage("Evidence type cannot exceed 150 characters.");
    }
}

