using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Resumes.Commands.DeleteResume;

public sealed class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteResumeCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
    {
        var resume = await _uow.Resumes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Resume", request.Id);

        await _uow.Resumes.DeleteAsync(resume.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

