using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.CreateCredential;

public record CreateCredentialCommand(
    string ServiceName,
    string Username,
    string Password,
    string? PortalUrl = null,
    string? Notes = null,
    Guid? CompanyId = null,
    Guid? JobApplicationId = null
) : IRequest<CredentialDto>;

public class CreateCredentialCommandHandler : IRequestHandler<CreateCredentialCommand, CredentialDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityKeyManager _keyManager;
    private readonly IEncryptionService _encryptionService;

    public CreateCredentialCommandHandler(
        IUnitOfWork unitOfWork,
        ISecurityKeyManager keyManager,
        IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _keyManager = keyManager;
        _encryptionService = encryptionService;
    }

    public async Task<CredentialDto> Handle(CreateCredentialCommand request, CancellationToken cancellationToken)
    {
        if (!_keyManager.IsVaultUnlocked())
        {
            throw new InvalidOperationException("Security vault is locked. Please unlock the vault with your master password first.");
        }

        var key = _keyManager.GetActiveEncryptionKey();
        string encryptedPassword = _encryptionService.Encrypt(request.Password, key);
        string? encryptedNotes = !string.IsNullOrWhiteSpace(request.Notes)
            ? _encryptionService.Encrypt(request.Notes, key)
            : null;

        var credential = CredentialItem.Create(
            request.ServiceName,
            request.Username,
            encryptedPassword,
            request.PortalUrl,
            encryptedNotes,
            request.CompanyId,
            request.JobApplicationId
        );

        await _unitOfWork.Credentials.AddAsync(credential, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CredentialDto
        {
            Id = credential.Id,
            ServiceName = credential.ServiceName,
            PortalUrl = credential.PortalUrl,
            Username = credential.Username,
            MaskedPassword = "••••••••••••",
            HasNotes = !string.IsNullOrWhiteSpace(credential.EncryptedNotes),
            CompanyId = credential.CompanyId,
            JobApplicationId = credential.JobApplicationId,
            LastUsedAt = credential.LastUsedAt,
            PasswordLastChangedAt = credential.PasswordLastChangedAt,
            CreatedAt = credential.CreatedAt
        };
    }
}

