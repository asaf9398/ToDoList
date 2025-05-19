using Common.Enums;
using System;

namespace Common.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        public bool IsCompleted { get; set; }
        public string LockedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
