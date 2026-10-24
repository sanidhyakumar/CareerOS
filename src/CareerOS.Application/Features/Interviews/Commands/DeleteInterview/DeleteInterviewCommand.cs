using CareerOS.Domain.Entities;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Interviews.Commands.DeleteInterview;

public record DeleteInterviewCommand(Guid Id) : IRequest<bool>;

public class DeleteInterviewCommandHandler : IRequestHandler<DeleteInterviewCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteInterviewCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _uow.Interviews.GetByIdAsync(request.Id, includeDetails: false, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Interview), request.Id);

        await _uow.Interviews.DeleteAsync(interview, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
