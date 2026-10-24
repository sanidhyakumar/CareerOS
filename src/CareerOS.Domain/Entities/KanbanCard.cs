using CareerOS.Domain.Common;
using CareerOS.Domain.Enums;

namespace CareerOS.Domain.Entities;

public class KanbanCard : BaseEntity
{
    public KanbanColumnType Column { get; set; } = KanbanColumnType.Interested;
    public int Position { get; set; }

    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? JobId { get; set; }
    public Job? Job { get; set; }

    public Guid? ApplicationId { get; set; }
    public JobApplication? Application { get; set; }

    public JobPriority Priority { get; set; } = JobPriority.Medium;
    public string? Salary { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public DateTime? DueDate { get; set; }

    public KanbanCard() { }

    public static KanbanCard Create(
        string title,
        string companyName,
        KanbanColumnType column = KanbanColumnType.Interested,
        int position = 0,
        Guid? companyId = null,
        Guid? jobId = null,
        Guid? applicationId = null,
        JobPriority priority = JobPriority.Medium,
        string? salary = null,
        string? location = null,
        string? notes = null,
        string? tags = null,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        return new KanbanCard
        {
            Title = title.Trim(),
            CompanyName = companyName?.Trim() ?? string.Empty,
            Column = column,
            Position = position,
            CompanyId = companyId,
            JobId = jobId,
            ApplicationId = applicationId,
            Priority = priority,
            Salary = salary?.Trim(),
            Location = location?.Trim(),
            Notes = notes?.Trim(),
            Tags = tags?.Trim(),
            DueDate = dueDate
        };
    }

    public void MoveTo(KanbanColumnType targetColumn, int newPosition)
    {
        Column = targetColumn;
        Position = newPosition;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string title,
        string companyName,
        KanbanColumnType column,
        int position,
        Guid? companyId,
        Guid? jobId,
        Guid? applicationId,
        JobPriority priority,
        string? salary,
        string? location,
        string? notes,
        string? tags,
        DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        Title = title.Trim();
        CompanyName = companyName?.Trim() ?? string.Empty;
        Column = column;
        Position = position;
        CompanyId = companyId;
        JobId = jobId;
        ApplicationId = applicationId;
        Priority = priority;
        Salary = salary?.Trim();
        Location = location?.Trim();
        Notes = notes?.Trim();
        Tags = tags?.Trim();
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
