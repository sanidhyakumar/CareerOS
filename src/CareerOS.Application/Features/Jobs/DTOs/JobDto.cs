using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Jobs.DTOs;

public class JobDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public JobStatus Status { get; set; }
    public string? Salary { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime? InterviewAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

