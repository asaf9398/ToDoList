
using Common.DTOs;

namespace ToDoListServer.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<TaskDto> AddAsync(TaskDto dto, string username);
        Task<TaskDto> UpdateAsync(TaskDto dto, string username);
        Task DeleteAsync(Guid id, string username);
        Task<bool> LockAsync(Guid taskId, string username);
        Task<bool> UnlockAsync(Guid taskId, string username);
    }
}
