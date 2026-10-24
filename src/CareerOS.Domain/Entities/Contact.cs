using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Contact : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ContactCategory Category { get; set; } = ContactCategory.Recruiter;

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Location { get; set; }

    public string? Company { get; set; }
    public Guid? CompanyId { get; set; }
    public Company? CompanyEntity { get; set; }

    public string? JobTitle { get; set; }
    public string? Department { get; set; }

    public Guid? JobId { get; set; }
    public Job? JobEntity { get; set; }

    public bool IsStarred { get; set; }
    public DateTime? LastContactDate { get; set; }
    public DateTime? NextFollowUpDate { get; set; }

    public string? RelationshipNotes { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }

    public ICollection<ContactInteraction> Interactions { get; set; } = new List<ContactInteraction>();

    public Contact() { }

    public static Contact Create(
        string name,
        ContactCategory category = ContactCategory.Recruiter,
        string? email = null,
        string? phone = null,
        string? company = null,
        Guid? companyId = null,
        string? jobTitle = null,
        string? department = null,
        string? linkedin = null,
        string? location = null,
        bool isStarred = false,
        string? notes = null,
        string? tags = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        return new Contact
        {
            Name = name.Trim(),
            Category = category,
            Email = email?.Trim(),
            Phone = phone?.Trim(),
            Company = company?.Trim(),
            CompanyId = companyId,
            JobTitle = jobTitle?.Trim(),
            Department = department?.Trim(),
            LinkedIn = linkedin?.Trim(),
            Location = location?.Trim(),
            IsStarred = isStarred,
            Notes = notes?.Trim(),
            Tags = tags?.Trim()
        };
    }

    public void Update(
        string name,
        ContactCategory category,
        string? email,
        string? phone,
        string? company,
        Guid? companyId,
        string? jobTitle,
        string? department,
        string? linkedin,
        string? location,
        bool isStarred,
        string? notes,
        string? tags,
        DateTime? nextFollowUpDate = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
        Category = category;
        Email = email?.Trim();
        Phone = phone?.Trim();
        Company = company?.Trim();
        CompanyId = companyId;
        JobTitle = jobTitle?.Trim();
        Department = department?.Trim();
        LinkedIn = linkedin?.Trim();
        Location = location?.Trim();
        IsStarred = isStarred;
        Notes = notes?.Trim();
        Tags = tags?.Trim();
        NextFollowUpDate = nextFollowUpDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
