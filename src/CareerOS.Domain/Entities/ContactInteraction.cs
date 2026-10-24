using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class ContactInteraction : BaseEntity
{
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;

    public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
    public string Channel { get; set; } = "Email"; // Email, LinkedIn, Phone, Coffee Chat, Video Call, Interview, In Person
    public string Summary { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool FollowUpRequired { get; set; }
    public DateTime? FollowUpDate { get; set; }

    public ContactInteraction() { }

    public static ContactInteraction Create(
        Guid contactId,
        string summary,
        DateTime interactionDate,
        string channel = "Email",
        string? notes = null,
        bool followUpRequired = false,
        DateTime? followUpDate = null)
    {
        if (string.IsNullOrWhiteSpace(summary)) throw new ArgumentException("Summary is required.", nameof(summary));

        return new ContactInteraction
        {
            ContactId = contactId,
            Summary = summary.Trim(),
            InteractionDate = interactionDate,
            Channel = string.IsNullOrWhiteSpace(channel) ? "Email" : channel.Trim(),
            Notes = notes?.Trim(),
            FollowUpRequired = followUpRequired,
            FollowUpDate = followUpDate
        };
    }
}
