using AutoMapper;
using CareerOS.Application.Features.Countries.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Countries.Commands.CreateCountry;

public record CreateCountryCommand(string Name, string Code) : IRequest<CountryDto>;

public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MinimumLength(2).MaximumLength(10);
    }
}

public sealed class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, CountryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCountryCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CountryDto> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Countries.GetByCodeAsync(request.Code, cancellationToken);
        if (existing != null)
        {
            throw new ArgumentException($"Country with code '{request.Code}' already exists.");
        }

        var country = Country.Create(request.Name, request.Code);
        await _uow.Countries.AddAsync(country, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CountryDto>(country);
    }
}

