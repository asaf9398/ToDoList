using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToDoListServer.Interfaces;
using ToDoListServer.Models;
using ToDoListServer.Services;
using Microsoft.Extensions.Logging;
using Common.DTOs;
using Common.Enums;

namespace ToDoListServer.Tests.Services
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _taskRepoMock = null!;
        private Mock<IAuditLogRepository> _auditRepoMock = null!;
        private Mock<ILogger<TaskService>> _loggerMock = null!;
        private TaskService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _auditRepoMock = new Mock<IAuditLogRepository>();
            _loggerMock = new Mock<ILogger<TaskService>>();
            _service = new TaskService(_taskRepoMock.Object, _auditRepoMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task AddAsync_ShouldAdd_WhenNotExists()
        {
            var dto = new TaskDto { Id = Guid.NewGuid(), Title = "New Task", CreatedAt = DateTime.UtcNow, Priority = TaskPriority.Low };
            _taskRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((TaskItem?)null);
            _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync((TaskItem t) => t);

            var result = await _service.AddAsync(dto, "user1");

            Assert.AreEqual(dto.Id, result.Id);
            _auditRepoMock.Verify(x => x.LogAsync(It.IsAny<TaskAuditLog>()), Times.Once);
        }

        [TestMethod]
        public async Task LockAsync_ShouldLock_WhenNotLocked()
        {
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, LockedBy = null };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);
            _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);

            var result = await _service.LockAsync(taskId, "user1");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDelete_WhenUnlocked()
        {
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, LockedBy = null };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            await _service.DeleteAsync(taskId, "user1");

            _taskRepoMock.Verify(r => r.DeleteAsync(taskId), Times.Once);
            _auditRepoMock.Verify(r => r.LogAsync(It.IsAny<TaskAuditLog>()), Times.Once);
        }
    }
}
