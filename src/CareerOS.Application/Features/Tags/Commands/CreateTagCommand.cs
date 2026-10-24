using AutoMapper;
using CareerOS.Application.Features.Tags.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CareerOS.Application.Features.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, string? ColorHex = null) : IRequest<TagDto>;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ColorHex).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.ColorHex));
    }
}

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateTagCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Tags.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null)
        {
            return _mapper.Map<TagDto>(existing);
        }

        var tag = Tag.Create(request.Name, request.ColorHex);
        await _uow.Tags.AddAsync(tag, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TagDto>(tag);
    }
}

public record DeleteTagCommand(Guid Id) : IRequest;

public sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteTagCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        await _uow.Tags.DeleteAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public record GetAllTagsQuery : IRequest<IEnumerable<TagDto>>;

public sealed class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllTagsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _uow.Tags.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }
}

