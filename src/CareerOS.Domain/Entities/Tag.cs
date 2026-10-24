using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ColorHex { get; set; }

    public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();

    public Tag() { }

    public static Tag Create(string name, string? colorHex = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name is required.", nameof(name));

        return new Tag
        {
            Name = name.Trim().ToLowerInvariant(),
            ColorHex = colorHex
        };
    }
}

