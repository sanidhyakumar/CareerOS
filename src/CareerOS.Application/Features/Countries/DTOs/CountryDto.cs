namespace CareerOS.Application.Features.Countries.DTOs;

public class CountryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int RegionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RegionDto
{
    public Guid Id { get; set; }
    public Guid CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

