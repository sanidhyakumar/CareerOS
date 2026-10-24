using AutoMapper;
using CareerOS.Application.Features.Kanban.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Kanban.Commands.UpdateKanbanCard;

public record UpdateKanbanCardCommand : IRequest<KanbanCardDto>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public KanbanColumnType Column { get; init; }
    public int Position { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? JobId { get; init; }
    public Guid? ApplicationId { get; init; }
    public JobPriority Priority { get; init; }
    public string? Salary { get; init; }
    public string? Location { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public DateTime? DueDate { get; init; }
}

public class UpdateKanbanCardCommandHandler : IRequestHandler<UpdateKanbanCardCommand, KanbanCardDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateKanbanCardCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<KanbanCardDto> Handle(UpdateKanbanCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _uow.Kanban.GetByIdAsync(request.Id, cancellationToken);
        if (card == null)
        {
            throw new NotFoundException(nameof(KanbanCard), request.Id);
        }

        card.Update(
            title: request.Title,
            companyName: request.CompanyName,
            column: request.Column,
            position: request.Position,
            companyId: request.CompanyId,
            jobId: request.JobId,
            applicationId: request.ApplicationId,
            priority: request.Priority,
            salary: request.Salary,
            location: request.Location,
            notes: request.Notes,
            tags: request.Tags,
            dueDate: request.DueDate);

        await _uow.Kanban.UpdateAsync(card, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KanbanCardDto>(card);
    }
}

public class UpdateKanbanCardCommandValidator : AbstractValidator<UpdateKanbanCardCommand>
{
    public UpdateKanbanCardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Card ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
    }
}

