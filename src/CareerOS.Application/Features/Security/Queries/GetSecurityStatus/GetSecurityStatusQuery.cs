using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Queries.GetSecurityStatus;

public record GetSecurityStatusQuery : IRequest<SecurityStatusDto>;

public class GetSecurityStatusQueryHandler : IRequestHandler<GetSecurityStatusQuery, SecurityStatusDto>
{
    private readonly ISecurityKeyManager _keyManager;
    private readonly IUnitOfWork _unitOfWork;

    public GetSecurityStatusQueryHandler(ISecurityKeyManager keyManager, IUnitOfWork unitOfWork)
    {
        _keyManager = keyManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<SecurityStatusDto> Handle(GetSecurityStatusQuery request, CancellationToken cancellationToken)
    {
        bool isConfigured = await _keyManager.IsMasterPasswordConfiguredAsync(cancellationToken);
        bool isUnlocked = _keyManager.IsVaultUnlocked();

        var credentials = await _unitOfWork.Credentials.GetAllAsync(null, cancellationToken);
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
        int needingReview = credentials.Count(c => c.PasswordLastChangedAt < ninetyDaysAgo);

        return new SecurityStatusDto
        {
            IsMasterPasswordConfigured = isConfigured,
            IsVaultUnlocked = isUnlocked,
            TotalCredentials = credentials.Count,
            PasswordsNeedingReview = needingReview
        };
    }
}
