using Common.DTOs;
using ToDoListServer.Interfaces;
using ToDoListServer.Models;

namespace ToDoListServer.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepo, IAuditLogRepository auditRepo, ILogger<TaskService> logger)
        {
            _taskRepo = taskRepo;
            _auditRepo = auditRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var tasks = await _taskRepo.GetAllAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto> AddAsync(TaskDto dto, string username)
        {
            var isAlreadyInDb = await _taskRepo.GetByIdAsync(dto.Id) != null ? true : false;

            if (isAlreadyInDb)
            {
                throw new Exception($"Task with id={dto.Id} is already in DB!");
            }

            var item = new TaskItem
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                IsCompleted = false,
                CreatedAt = dto.CreatedAt,
            };

            var result = await _taskRepo.AddAsync(item);
            await _auditRepo.LogAsync(new TaskAuditLog { TaskId = result.Id, Action = "Add", Username = username, Description = dto.Description });
            _logger.LogInformation($"Task {result.Id} added by {username}");
            return MapToDto(result);
        }

        public async Task<TaskDto> UpdateAsync(TaskDto dto, string username)
        {
            var item = await _taskRepo.GetByIdAsync(dto.Id);

            if (item == null)
            {
                throw new Exception("Task not found");
            }

            item.Title = dto.Title;
            item.Description = dto.Description;
            item.Priority = dto.Priority;
            item.IsCompleted = dto.IsCompleted;

            if (item.LockedBy != null && item.LockedBy != username)
            {
                throw new Exception("Task is locked by other user!");
            }

            var result = await _taskRepo.UpdateAsync(item);
            await _auditRepo.LogAsync(new TaskAuditLog { TaskId = result.Id, Action = "Update", Username = username, Description = dto.Description });
            _logger.LogInformation($"Task {result.Id} updated by {username}");

            return MapToDto(result);
        }

        public async Task DeleteAsync(Guid id, string username)
        {
            var item = await _taskRepo.GetByIdAsync(id);

            if (item == null)
            {
                throw new Exception("Task not found");
            }

            if (item.LockedBy != null && item.LockedBy != username)
            {
                throw new Exception("Task is locked by other user!");
            }

            await _taskRepo.DeleteAsync(id);
            await _auditRepo.LogAsync(new TaskAuditLog { TaskId = id, Action = "Delete", Username = username });
            _logger.LogWarning($"Task {id} deleted by {username}");
        }

        public async Task<bool> LockAsync(Guid taskId, string username)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new Exception("Task not found");
            }

            if (!string.IsNullOrEmpty(task.LockedBy))
            {
                return false;
            }

            task.LockedBy = username;
            task.LockTimestamp = DateTime.UtcNow;
            await _taskRepo.UpdateAsync(task);
            _logger.LogInformation($"Task {taskId} locked by {username}");

            return true;
        }

        public async Task<bool> UnlockAsync(Guid taskId, string username)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new Exception("Task not found");
            }

            if (task.LockedBy != username && task.LockedBy != null)
            {
                return false;
            }

            task.LockedBy = null;
            task.LockTimestamp = null;
            await _taskRepo.UpdateAsync(task);
            _logger.LogInformation($"Task {taskId} unlocked by {username}");

            return true;
        }

        private TaskDto MapToDto(TaskItem item) => new()
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Priority = item.Priority,
            IsCompleted = item.IsCompleted,
            LockedBy = item.LockedBy,
            CreatedAt = item.CreatedAt,
        };
    }
}
