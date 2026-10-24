using CareerOS.Application.Features.Security.DTOs;
using CareerOS.Domain.Interfaces;
using MediatR;

namespace CareerOS.Application.Features.Security.Queries.GetCredentials;

public record GetCredentialsQuery(Guid? CompanyId = null) : IRequest<List<CredentialDto>>;

public class GetCredentialsQueryHandler : IRequestHandler<GetCredentialsQuery, List<CredentialDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCredentialsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CredentialDto>> Handle(GetCredentialsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Credentials.GetAllAsync(request.CompanyId, cancellationToken);

        return items.Select(c => new CredentialDto
        {
            Id = c.Id,
            ServiceName = c.ServiceName,
            PortalUrl = c.PortalUrl,
            Username = c.Username,
            MaskedPassword = "••••••••••••",
            HasNotes = !string.IsNullOrWhiteSpace(c.EncryptedNotes),
            CompanyId = c.CompanyId,
            CompanyName = c.Company?.Name,
            JobApplicationId = c.JobApplicationId,
            JobTitle = c.JobApplication?.Job?.Title,
            LastUsedAt = c.LastUsedAt,
            PasswordLastChangedAt = c.PasswordLastChangedAt,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}

