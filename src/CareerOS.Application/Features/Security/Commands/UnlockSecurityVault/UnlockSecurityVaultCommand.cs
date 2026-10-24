using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.UnlockSecurityVault;

public record UnlockSecurityVaultCommand(string MasterPassword) : IRequest<bool>;

public class UnlockSecurityVaultCommandHandler : IRequestHandler<UnlockSecurityVaultCommand, bool>
{
    private readonly ISecurityKeyManager _keyManager;

    public UnlockSecurityVaultCommandHandler(ISecurityKeyManager keyManager)
    {
        _keyManager = keyManager;
    }

    public async Task<bool> Handle(UnlockSecurityVaultCommand request, CancellationToken cancellationToken)
    {
        return await _keyManager.UnlockVaultAsync(request.MasterPassword, cancellationToken);
    }
}

