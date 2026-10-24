using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Commands.UpdateSponsorshipEvidence;

public record UpdateSponsorshipEvidenceCommand : IRequest<SponsorshipEvidenceDto>
{
    public Guid Id { get; init; }
    public string EvidenceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? DocumentId { get; init; }
    public bool IsVerified { get; init; }
    public DateTime? ExpiryDate { get; init; }
}

public class UpdateSponsorshipEvidenceCommandHandler : IRequestHandler<UpdateSponsorshipEvidenceCommand, SponsorshipEvidenceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateSponsorshipEvidenceCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SponsorshipEvidenceDto> Handle(UpdateSponsorshipEvidenceCommand request, CancellationToken cancellationToken)
    {
        var evidence = await _uow.Sponsorship.GetEvidenceByIdAsync(request.Id, cancellationToken);
        if (evidence == null)
        {
            throw new NotFoundException(nameof(SponsorshipEvidence), request.Id);
        }

        evidence.Update(
            evidenceType: request.EvidenceType,
            description: request.Description,
            documentId: request.DocumentId,
            isVerified: request.IsVerified,
            expiryDate: request.ExpiryDate);

        await _uow.Sponsorship.UpdateEvidenceAsync(evidence, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var loaded = await _uow.Sponsorship.GetEvidenceByIdAsync(evidence.Id, cancellationToken);
        return _mapper.Map<SponsorshipEvidenceDto>(loaded ?? evidence);
    }
}
