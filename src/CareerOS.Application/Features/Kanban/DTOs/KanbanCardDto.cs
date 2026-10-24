using CareerOS.Domain.Enums;

namespace CareerOS.Application.Features.Kanban.DTOs;

public class KanbanCardDto
{
    public Guid Id { get; set; }
    public KanbanColumnType Column { get; set; }
    public string ColumnName => Column.ToString();
    public int Position { get; set; }

    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public Guid? CompanyId { get; set; }
    public Guid? JobId { get; set; }
    public Guid? ApplicationId { get; set; }

    public JobPriority Priority { get; set; }
    public string PriorityName => Priority.ToString();

    public string? Salary { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public List<string> TagsList => !string.IsNullOrWhiteSpace(Tags)
        ? Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        : new();

    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class KanbanColumnDto
{
    public KanbanColumnType ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<KanbanCardDto> Cards { get; set; } = new();
}

public class KanbanBoardDto
{
    public List<KanbanColumnDto> Columns { get; set; } = new();
    public int TotalCards { get; set; }
}
