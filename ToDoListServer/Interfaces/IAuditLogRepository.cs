using ToDoListServer.Models;

namespace ToDoListServer.Interfaces
{
    public interface IAuditLogRepository
    {
        Task LogAsync(TaskAuditLog log);
    }
}
