using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Contacts.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ContactCategory Category { get; set; }
    public string CategoryName => Category.ToString();

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Location { get; set; }

    public string? Company { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string? JobTitle { get; set; }
    public string? Department { get; set; }

    public Guid? JobId { get; set; }
    public string? PositionTitle { get; set; }

    public bool IsStarred { get; set; }
    public DateTime? LastContactDate { get; set; }
    public DateTime? NextFollowUpDate { get; set; }

    public string? RelationshipNotes { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }

    public int InteractionsCount { get; set; }
    public List<ContactInteractionDto> Interactions { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ContactInteractionDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public DateTime InteractionDate { get; set; }
    public string Channel { get; set; } = "Email";
    public string Summary { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool FollowUpRequired { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactStatsDto
{
    public int TotalContacts { get; set; }
    public int RecruitersCount { get; set; }
    public int HiringManagersCount { get; set; }
    public int HumanResourcesCount { get; set; }
    public int ReferralsCount { get; set; }
    public int ImmigrationCount { get; set; }
    public int FollowUpsDueCount { get; set; }
    public int StarredCount { get; set; }
}

