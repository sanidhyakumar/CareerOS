using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Profile.DTOs;

public class SocialLinkDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public SocialPlatform Platform { get; set; }
    public string PlatformName => Platform.ToString();
    public string Url { get; set; } = string.Empty;
    public string? DisplayLabel { get; set; }
    public int DisplayOrder { get; set; }
}

