using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Commands.UpdateCredential;

public record UpdateCredentialCommand(
    Guid Id,
    string ServiceName,
    string Username,
    string? Password = null,
    string? PortalUrl = null,
    string? Notes = null,
    Guid? CompanyId = null
) : IRequest<CredentialDto>;

public class UpdateCredentialCommandHandler : IRequestHandler<UpdateCredentialCommand, CredentialDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityKeyManager _keyManager;
    private readonly IEncryptionService _encryptionService;

    public UpdateCredentialCommandHandler(
        IUnitOfWork unitOfWork,
        ISecurityKeyManager keyManager,
        IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _keyManager = keyManager;
        _encryptionService = encryptionService;
    }

    public async Task<CredentialDto> Handle(UpdateCredentialCommand request, CancellationToken cancellationToken)
    {
        if (!_keyManager.IsVaultUnlocked())
        {
            throw new InvalidOperationException("Security vault is locked. Please unlock the vault with your master password first.");
        }

        var item = await _unitOfWork.Credentials.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(item), request.Id);
        }

        var key = _keyManager.GetActiveEncryptionKey();

        item.ServiceName = request.ServiceName.Trim();
        item.Username = request.Username.Trim();
        item.PortalUrl = request.PortalUrl?.Trim();
        item.CompanyId = request.CompanyId;
        item.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            item.EncryptedPassword = _encryptionService.Encrypt(request.Password, key);
            item.PasswordLastChangedAt = DateTime.UtcNow;
        }

        if (request.Notes != null)
        {
            item.EncryptedNotes = !string.IsNullOrWhiteSpace(request.Notes)
                ? _encryptionService.Encrypt(request.Notes, key)
                : null;
        }

        await _unitOfWork.Credentials.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CredentialDto
        {
            Id = item.Id,
            ServiceName = item.ServiceName,
            PortalUrl = item.PortalUrl,
            Username = item.Username,
            MaskedPassword = "••••••••••••",
            HasNotes = !string.IsNullOrWhiteSpace(item.EncryptedNotes),
            CompanyId = item.CompanyId,
            CompanyName = item.Company?.Name,
            JobApplicationId = item.JobApplicationId,
            LastUsedAt = item.LastUsedAt,
            PasswordLastChangedAt = item.PasswordLastChangedAt,
            CreatedAt = item.CreatedAt
        };
    }
}
