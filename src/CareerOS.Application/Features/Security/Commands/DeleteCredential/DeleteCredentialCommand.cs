using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.DeleteCredential;

public record DeleteCredentialCommand(Guid Id) : IRequest<bool>;

public class DeleteCredentialCommandHandler : IRequestHandler<DeleteCredentialCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCredentialCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCredentialCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Credentials.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(item), request.Id);
        }

        await _unitOfWork.Credentials.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

