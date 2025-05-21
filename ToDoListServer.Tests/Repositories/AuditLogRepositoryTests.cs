using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ToDoListServer.Data;
using ToDoListServer.Models;
using ToDoListServer.Repositories;

namespace ToDoListServer.Tests.Repositories
{
    [TestClass]
    public class AuditLogRepositoryTests
    {
        private TasksDbContext _context = null!;
        private AuditLogRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
                .UseInMemoryDatabase("TestDB_Audit")
                .Options;
            _context = new TasksDbContext(options);
            _repo = new AuditLogRepository(_context);
        }

        [TestMethod]
        public async Task LogAsync_ShouldInsertAuditLog()
        {
            var log = new TaskAuditLog { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), Action = "Test", Username = "user" };
            await _repo.LogAsync(log);
            var count = await _context.TaskAuditLogs.CountAsync();
            Assert.AreEqual(1, count);
        }
    }
}
