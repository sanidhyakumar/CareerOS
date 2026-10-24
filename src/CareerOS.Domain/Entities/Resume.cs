using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class Resume : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Version { get; set; } = 1;
    public bool IsDefault { get; set; }
    public string? Tags { get; set; }

    public Resume() { }

    public static Resume Create(string name, string? content = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        return new Resume
        {
            Name = name,
            Content = content
        };
    }

    public void Update(string name, string? content, bool isDefault, string? tags)
    {
        Name = name;
        Content = content;
        IsDefault = isDefault;
        Tags = tags;
        UpdatedAt = DateTime.UtcNow;
    }
}

