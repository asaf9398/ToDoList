using ToDoListServer.Enums;

namespace ToDoListServer.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LockedBy { get; set; }
        public DateTime? LockTimestamp { get; set; }
    }
}
