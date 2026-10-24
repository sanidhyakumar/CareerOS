using AutoMapper;
using CareerOS.Domain.Exceptions;
using CareerOS.Application.Features.ResumeCenter.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.ResumeCenter.Commands.Portfolios;

public record CreatePortfolioVersionCommand(
    string Name,
    string? Url = null,
    string? TargetRole = null,
    string? Description = null,
    string? FeaturedProjects = null,
    bool IsDefault = false) : IRequest<PortfolioVersionDto>;

public class CreatePortfolioVersionCommandValidator : AbstractValidator<CreatePortfolioVersionCommand>
{
    public CreatePortfolioVersionCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class CreatePortfolioVersionCommandHandler : IRequestHandler<CreatePortfolioVersionCommand, PortfolioVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreatePortfolioVersionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PortfolioVersionDto> Handle(CreatePortfolioVersionCommand request, CancellationToken cancellationToken)
    {
        var portfolio = PortfolioVersion.Create(
            request.Name,
            request.Url,
            request.TargetRole,
            request.Description,
            request.FeaturedProjects,
            request.IsDefault);

        await _uow.ResumeCenter.AddPortfolioVersionAsync(portfolio, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PortfolioVersionDto>(portfolio);
    }
}

public record UpdatePortfolioVersionCommand(
    Guid Id,
    string Name,
    string? Url,
    string? TargetRole,
    string? Description,
    string? FeaturedProjects,
    bool IsDefault) : IRequest<PortfolioVersionDto>;

public class UpdatePortfolioVersionCommandValidator : AbstractValidator<UpdatePortfolioVersionCommand>
{
    public UpdatePortfolioVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class UpdatePortfolioVersionCommandHandler : IRequestHandler<UpdatePortfolioVersionCommand, PortfolioVersionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdatePortfolioVersionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PortfolioVersionDto> Handle(UpdatePortfolioVersionCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _uow.ResumeCenter.GetPortfolioVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PortfolioVersion), request.Id);

        portfolio.Update(
            request.Name,
            request.Url,
            request.TargetRole,
            request.Description,
            request.FeaturedProjects,
            request.IsDefault);

        await _uow.ResumeCenter.UpdatePortfolioVersionAsync(portfolio, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PortfolioVersionDto>(portfolio);
    }
}

public record DeletePortfolioVersionCommand(Guid Id) : IRequest<Unit>;

public class DeletePortfolioVersionCommandHandler : IRequestHandler<DeletePortfolioVersionCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeletePortfolioVersionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Unit> Handle(DeletePortfolioVersionCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _uow.ResumeCenter.GetPortfolioVersionByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PortfolioVersion), request.Id);

        await _uow.ResumeCenter.DeletePortfolioVersionAsync(portfolio, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

