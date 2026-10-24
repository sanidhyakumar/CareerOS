using AutoMapper;
using CareerOS.Application.Features.Companies.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Companies.Commands;

public record CreateCompanyCommand(
    string Name,
    string? Description = null,
    string? Headquarters = null,
    string? Size = null,
    string? Stage = null,
    int? FoundedYear = null,
    string? LogoUrl = null,
    string? WebsiteUrl = null,
    string? CareersUrl = null,
    string? TechBlogUrl = null,
    string? LinkedInUrl = null,
    string? GlassdoorUrl = null,
    decimal? GlassdoorRating = null,
    int PersonalRating = 3,
    int CultureRating = 3,
    int WorkLifeBalanceRating = 3,
    SponsorshipStatus SponsorshipStatus = SponsorshipStatus.Unknown,
    string? SponsoredVisas = null,
    string? SponsorshipNotes = null,
    RelocationStatus RelocationStatus = RelocationStatus.Unknown,
    string? RelocationNotes = null,
    string? Industry = null,
    string? Sector = null,
    string? Tags = null,
    string? Notes = null,
    string? Pros = null,
    string? Cons = null,
    bool IsTarget = false) : IRequest<CompanyDto>;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PersonalRating).InclusiveBetween(1, 5);
        RuleFor(x => x.CultureRating).InclusiveBetween(1, 5);
        RuleFor(x => x.WorkLifeBalanceRating).InclusiveBetween(1, 5);
    }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCompanyCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Companies.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Company with name '{request.Name}' already exists.");
        }

        var company = Company.Create(
            request.Name,
            request.Description,
            request.Headquarters,
            request.Industry,
            request.WebsiteUrl,
            request.CareersUrl,
            request.LinkedInUrl,
            request.GlassdoorUrl,
            request.GlassdoorRating,
            request.SponsorshipStatus,
            request.SponsoredVisas,
            request.RelocationStatus,
            request.PersonalRating,
            request.IsTarget);

        company.Size = request.Size?.Trim();
        company.Stage = request.Stage?.Trim();
        company.FoundedYear = request.FoundedYear;
        company.LogoUrl = request.LogoUrl?.Trim();
        company.TechBlogUrl = request.TechBlogUrl?.Trim();
        company.CultureRating = Math.Clamp(request.CultureRating, 1, 5);
        company.WorkLifeBalanceRating = Math.Clamp(request.WorkLifeBalanceRating, 1, 5);
        company.SponsorshipNotes = request.SponsorshipNotes?.Trim();
        company.RelocationNotes = request.RelocationNotes?.Trim();
        company.Sector = request.Sector?.Trim();
        company.Tags = request.Tags?.Trim();
        company.Notes = request.Notes?.Trim();
        company.Pros = request.Pros?.Trim();
        company.Cons = request.Cons?.Trim();

        await _uow.Companies.AddAsync(company, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CompanyDto>(company);
    }
}

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Headquarters,
    string? Size,
    string? Stage,
    int? FoundedYear,
    string? LogoUrl,
    string? WebsiteUrl,
    string? CareersUrl,
    string? TechBlogUrl,
    string? LinkedInUrl,
    string? GlassdoorUrl,
    decimal? GlassdoorRating,
    int PersonalRating,
    int CultureRating,
    int WorkLifeBalanceRating,
    SponsorshipStatus SponsorshipStatus,
    string? SponsoredVisas,
    string? SponsorshipNotes,
    RelocationStatus RelocationStatus,
    string? RelocationNotes,
    string? Industry,
    string? Sector,
    string? Tags,
    string? Notes,
    string? Pros,
    string? Cons,
    bool IsTarget) : IRequest<CompanyDto>;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PersonalRating).InclusiveBetween(1, 5);
        RuleFor(x => x.CultureRating).InclusiveBetween(1, 5);
        RuleFor(x => x.WorkLifeBalanceRating).InclusiveBetween(1, 5);
    }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateCompanyCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _uow.Companies.GetByIdAsync(request.Id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.Id);

        company.Update(
            request.Name,
            request.Description,
            request.Headquarters,
            request.Size,
            request.Stage,
            request.FoundedYear,
            request.LogoUrl,
            request.WebsiteUrl,
            request.CareersUrl,
            request.TechBlogUrl,
            request.LinkedInUrl,
            request.GlassdoorUrl,
            request.GlassdoorRating,
            request.PersonalRating,
            request.CultureRating,
            request.WorkLifeBalanceRating,
            request.SponsorshipStatus,
            request.SponsoredVisas,
            request.SponsorshipNotes,
            request.RelocationStatus,
            request.RelocationNotes,
            request.Industry,
            request.Sector,
            request.Tags,
            request.Notes,
            request.Pros,
            request.Cons,
            request.IsTarget);

        await _uow.Companies.UpdateAsync(company, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CompanyDto>(company);
    }
}

public record DeleteCompanyCommand(Guid Id) : IRequest<Unit>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteCompanyCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _uow.Companies.GetByIdAsync(request.Id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.Id);

        await _uow.Companies.DeleteAsync(company.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

