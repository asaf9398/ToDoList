using Microsoft.EntityFrameworkCore;
using ToDoListServer.Data;
using ToDoListServer.Interfaces;
using ToDoListServer.Models;

namespace ToDoListServer.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TasksDbContext _context;
        public TaskRepository(TasksDbContext context) => _context = context;

        public async Task<IEnumerable<TaskItem>> GetAllAsync() => await _context.Tasks.ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(Guid id) => await _context.Tasks.FindAsync(id);

        public async Task<TaskItem> AddAsync(TaskItem item)
        {
            _context.Tasks.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem item)
        {
            _context.Tasks.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
