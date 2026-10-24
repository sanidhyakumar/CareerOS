using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class Contact : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string? LinkedIn { get; set; }
    public string? Notes { get; set; }

    public Contact() { }

    public static Contact Create(string name, string? email = null, string? phone = null, string? company = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        return new Contact
        {
            Name = name,
            Email = email,
            Phone = phone,
            Company = company
        };
    }

    public void Update(string name, string? email, string? phone, string? company, string? jobTitle,
        string? linkedin, string? notes)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Company = company;
        JobTitle = jobTitle;
        LinkedIn = linkedin;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}

