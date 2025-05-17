using ToDoListServer.Models;

namespace ToDoListServer.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> AddAsync(TaskItem item);
        Task<TaskItem> UpdateAsync(TaskItem item);
        Task DeleteAsync(Guid id);
    }
}
