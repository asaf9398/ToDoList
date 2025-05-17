using System.Collections.Generic;
using ToDoListServer.Models;
using Microsoft.EntityFrameworkCore;


namespace ToDoListServer.Data
{
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskAuditLog> TaskAuditLogs => Set<TaskAuditLog>();
    }
}
