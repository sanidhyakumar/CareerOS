using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Commands.RemoveSponsorshipEvidence;

public record RemoveSponsorshipEvidenceCommand(Guid EvidenceId) : IRequest;

public class RemoveSponsorshipEvidenceCommandHandler : IRequestHandler<RemoveSponsorshipEvidenceCommand>
{
    private readonly IUnitOfWork _uow;

    public RemoveSponsorshipEvidenceCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(RemoveSponsorshipEvidenceCommand request, CancellationToken cancellationToken)
    {
        var evidence = await _uow.Sponsorship.GetEvidenceByIdAsync(request.EvidenceId, cancellationToken);
        if (evidence == null)
        {
            throw new NotFoundException(nameof(SponsorshipEvidence), request.EvidenceId);
        }

        await _uow.Sponsorship.DeleteEvidenceAsync(evidence, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

