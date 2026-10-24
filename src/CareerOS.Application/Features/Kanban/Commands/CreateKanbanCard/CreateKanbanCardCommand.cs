using AutoMapper;
using CareerOS.Application.Features.Kanban.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Enums;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Kanban.Commands.CreateKanbanCard;

public record CreateKanbanCardCommand : IRequest<KanbanCardDto>
{
    public string Title { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public KanbanColumnType Column { get; init; } = KanbanColumnType.Interested;
    public int Position { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? JobId { get; init; }
    public Guid? ApplicationId { get; init; }
    public JobPriority Priority { get; init; } = JobPriority.Medium;
    public string? Salary { get; init; }
    public string? Location { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public DateTime? DueDate { get; init; }
}

public class CreateKanbanCardCommandHandler : IRequestHandler<CreateKanbanCardCommand, KanbanCardDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateKanbanCardCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<KanbanCardDto> Handle(CreateKanbanCardCommand request, CancellationToken cancellationToken)
    {
        var card = KanbanCard.Create(
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

        await _uow.Kanban.AddAsync(card, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KanbanCardDto>(card);
    }
}

public class CreateKanbanCardCommandValidator : AbstractValidator<CreateKanbanCardCommand>
{
    public CreateKanbanCardCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
    }
}
