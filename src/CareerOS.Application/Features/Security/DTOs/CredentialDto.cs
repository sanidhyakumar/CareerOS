namespace CareerOS.Application.Features.Security.DTOs;

public class CredentialDto
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? PortalUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string MaskedPassword { get; set; } = "••••••••••••";
    public bool HasNotes { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? JobApplicationId { get; set; }
    public string? JobTitle { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime PasswordLastChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DecryptedCredentialDto
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? PortalUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? JobApplicationId { get; set; }
}

public class SecurityStatusDto
{
    public bool IsMasterPasswordConfigured { get; set; }
    public bool IsVaultUnlocked { get; set; }
    public int TotalCredentials { get; set; }
    public int PasswordsNeedingReview { get; set; }
}
