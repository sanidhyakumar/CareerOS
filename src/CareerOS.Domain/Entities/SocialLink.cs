using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class SocialLink : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public SocialPlatform Platform { get; set; } = SocialPlatform.LinkedIn;
    public string Url { get; set; } = string.Empty;
    public string? DisplayLabel { get; set; }
    public int DisplayOrder { get; set; }

    public SocialLink() { }

    public static SocialLink Create(
        Guid userProfileId,
        SocialPlatform platform,
        string url,
        string? displayLabel = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url is required.", nameof(url));

        return new SocialLink
        {
            UserProfileId = userProfileId,
            Platform = platform,
            Url = url.Trim(),
            DisplayLabel = displayLabel?.Trim(),
            DisplayOrder = displayOrder
        };
    }
}

