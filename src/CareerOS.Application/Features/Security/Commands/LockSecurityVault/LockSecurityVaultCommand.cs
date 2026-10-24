using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.LockSecurityVault;

public record LockSecurityVaultCommand : IRequest<bool>;

public class LockSecurityVaultCommandHandler : IRequestHandler<LockSecurityVaultCommand, bool>
{
    private readonly ISecurityKeyManager _keyManager;

    public LockSecurityVaultCommandHandler(ISecurityKeyManager keyManager)
    {
        _keyManager = keyManager;
    }

    public Task<bool> Handle(LockSecurityVaultCommand request, CancellationToken cancellationToken)
    {
        _keyManager.LockVault();
        return Task.FromResult(true);
    }
}

