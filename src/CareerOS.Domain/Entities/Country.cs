using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Region> Regions { get; set; } = new List<Region>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public Country() { }

    public static Country Create(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));

        return new Country
        {
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }
}

