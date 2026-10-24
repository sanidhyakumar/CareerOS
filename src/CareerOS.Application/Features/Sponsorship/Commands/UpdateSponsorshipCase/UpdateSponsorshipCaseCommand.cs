using AutoMapper;
using CareerOS.Application.Features.Sponsorship.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Sponsorship.Commands.UpdateSponsorshipCase;

public record UpdateSponsorshipCaseCommand : IRequest<SponsorshipCaseDto>
{
    public Guid Id { get; init; }
    public Guid CountryId { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public VisaProgramType ProgramType { get; init; }
    public SponsorshipEligibilityStatus Eligibility { get; init; }
    public VisaAvailabilityStatus Availability { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? JobId { get; init; }
    public decimal? MinimumSalaryRequirement { get; init; }
    public string? Currency { get; init; }
    public DateTime? NextFiscalCycleDate { get; init; }
    public DateTime? FilingDeadline { get; init; }
    public int? EstimatedProcessingMonths { get; init; }
    public bool SupportsDualIntent { get; init; }
    public bool LeadsToPermanentResidency { get; init; }
    public string? Notes { get; init; }
}

public class UpdateSponsorshipCaseCommandHandler : IRequestHandler<UpdateSponsorshipCaseCommand, SponsorshipCaseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateSponsorshipCaseCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<SponsorshipCaseDto> Handle(UpdateSponsorshipCaseCommand request, CancellationToken cancellationToken)
    {
        var sponsorship = await _uow.Sponsorship.GetByIdAsync(request.Id, includeDetails: true, cancellationToken: cancellationToken);
        if (sponsorship == null)
        {
            throw new NotFoundException(nameof(SponsorshipCase), request.Id);
        }

        sponsorship.Update(
            countryId: request.CountryId,
            programName: request.ProgramName,
            programType: request.ProgramType,
            eligibility: request.Eligibility,
            availability: request.Availability,
            companyId: request.CompanyId,
            jobId: request.JobId,
            minimumSalaryRequirement: request.MinimumSalaryRequirement,
            currency: request.Currency,
            nextFiscalCycleDate: request.NextFiscalCycleDate,
            filingDeadline: request.FilingDeadline,
            estimatedProcessingMonths: request.EstimatedProcessingMonths,
            supportsDualIntent: request.SupportsDualIntent,
            leadsToPermanentResidency: request.LeadsToPermanentResidency,
            notes: request.Notes);

        await _uow.Sponsorship.UpdateAsync(sponsorship, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var loaded = await _uow.Sponsorship.GetByIdAsync(sponsorship.Id, includeDetails: true, cancellationToken: cancellationToken);
        return _mapper.Map<SponsorshipCaseDto>(loaded ?? sponsorship);
    }
}

public class UpdateSponsorshipCaseCommandValidator : AbstractValidator<UpdateSponsorshipCaseCommand>
{
    public UpdateSponsorshipCaseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.ProgramName)
            .NotEmpty().WithMessage("Program name is required.")
            .MaximumLength(200).WithMessage("Program name cannot exceed 200 characters.");
    }
}

