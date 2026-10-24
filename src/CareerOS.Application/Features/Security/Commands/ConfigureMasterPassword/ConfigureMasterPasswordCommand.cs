using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.ConfigureMasterPassword;

public record ConfigureMasterPasswordCommand(string MasterPassword) : IRequest<bool>;

public class ConfigureMasterPasswordCommandHandler : IRequestHandler<ConfigureMasterPasswordCommand, bool>
{
    private readonly ISecurityKeyManager _keyManager;

    public ConfigureMasterPasswordCommandHandler(ISecurityKeyManager keyManager)
    {
        _keyManager = keyManager;
    }

    public async Task<bool> Handle(ConfigureMasterPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MasterPassword) || request.MasterPassword.Length < 6)
        {
            throw new ArgumentException("Master password must be at least 6 characters long.");
        }

        return await _keyManager.SetupMasterPasswordAsync(request.MasterPassword, cancellationToken);
    }
}

