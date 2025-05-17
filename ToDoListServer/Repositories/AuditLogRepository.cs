using ToDoListServer.Data;
using ToDoListServer.Interfaces;
using ToDoListServer.Models;

namespace ToDoListServer.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly TasksDbContext _context;
        public AuditLogRepository(TasksDbContext context) => _context = context;

        public async Task LogAsync(TaskAuditLog log)
        {
            _context.TaskAuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
