using Project.Domain.Entities.Enums;
using TaskStatus = Project.Domain.Entities.Enums.TaskStatus;

namespace Project.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Tags { get; set; } // Comma-separated or JSON

    // Foreign Keys
    public Guid UserId { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
}
