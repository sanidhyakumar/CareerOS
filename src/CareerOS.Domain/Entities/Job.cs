using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Saved;
    public string? Salary { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime? InterviewAt { get; set; }

    public Job() { }

    public static Job Create(string title, string company, string? location = null, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(company)) throw new ArgumentException("Company is required.", nameof(company));

        return new Job
        {
            Title = title,
            Company = company,
            Location = location,
            Url = url
        };
    }

    public void Update(string title, string company, string? location, string? url, string? description,
        string? notes, JobStatus status, string? salary, DateTime? appliedAt, DateTime? interviewAt)
    {
        Title = title;
        Company = company;
        Location = location;
        Url = url;
        Description = description;
        Notes = notes;
        Status = status;
        Salary = salary;
        AppliedAt = appliedAt;
        InterviewAt = interviewAt;
        UpdatedAt = DateTime.UtcNow;
    }
}

