using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class CredentialItem : BaseEntity
{
    public Guid? CareerProfileId { get; set; }
    public CareerProfile? CareerProfile { get; set; }

    public string ServiceName { get; set; } = string.Empty;
    public string? PortalUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string? EncryptedNotes { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? JobApplicationId { get; set; }
    public JobApplication? JobApplication { get; set; }

    public DateTime? LastUsedAt { get; set; }
    public DateTime PasswordLastChangedAt { get; set; } = DateTime.UtcNow;

    public CredentialItem() { }

    public static CredentialItem Create(
        string serviceName,
        string username,
        string encryptedPassword,
        string? portalUrl = null,
        string? encryptedNotes = null,
        Guid? companyId = null,
        Guid? jobApplicationId = null)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name is required.", nameof(serviceName));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new ArgumentException("Encrypted password is required.", nameof(encryptedPassword));

        return new CredentialItem
        {
            ServiceName = serviceName.Trim(),
            Username = username.Trim(),
            EncryptedPassword = encryptedPassword,
            PortalUrl = portalUrl?.Trim(),
            EncryptedNotes = encryptedNotes,
            CompanyId = companyId,
            JobApplicationId = jobApplicationId,
            PasswordLastChangedAt = DateTime.UtcNow
        };
    }
}
