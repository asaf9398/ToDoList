namespace ToDoListServer.Models
{
    public class TaskAuditLog
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
    }
}
