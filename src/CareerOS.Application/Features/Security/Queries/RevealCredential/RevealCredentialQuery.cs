using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Domain.Exceptions;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Queries.RevealCredential;

public record RevealCredentialQuery(Guid Id) : IRequest<DecryptedCredentialDto>;

public class RevealCredentialQueryHandler : IRequestHandler<RevealCredentialQuery, DecryptedCredentialDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityKeyManager _keyManager;
    private readonly IEncryptionService _encryptionService;

    public RevealCredentialQueryHandler(
        IUnitOfWork unitOfWork,
        ISecurityKeyManager keyManager,
        IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _keyManager = keyManager;
        _encryptionService = encryptionService;
    }

    public async Task<DecryptedCredentialDto> Handle(RevealCredentialQuery request, CancellationToken cancellationToken)
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
        string decryptedPassword = _encryptionService.Decrypt(item.EncryptedPassword, key);
        string? decryptedNotes = !string.IsNullOrWhiteSpace(item.EncryptedNotes)
            ? _encryptionService.Decrypt(item.EncryptedNotes, key)
            : null;

        item.LastUsedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DecryptedCredentialDto
        {
            Id = item.Id,
            ServiceName = item.ServiceName,
            PortalUrl = item.PortalUrl,
            Username = item.Username,
            Password = decryptedPassword,
            Notes = decryptedNotes,
            CompanyId = item.CompanyId,
            CompanyName = item.Company?.Name,
            JobApplicationId = item.JobApplicationId
        };
    }
}

