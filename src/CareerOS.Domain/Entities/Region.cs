using CareerOS.Domain.Common;

namespace CareerOS.Domain.Entities;

public class Region : BaseEntity
{
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public Region() { }

    public static Region Create(Guid countryId, string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));

        return new Region
        {
            CountryId = countryId,
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }
}

