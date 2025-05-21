using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ToDoListServer.Data;
using ToDoListServer.Models;
using ToDoListServer.Repositories;

namespace ToDoListServer.Tests.Repositories
{
    [TestClass]
    public class TaskRepositoryTests
    {
        private TasksDbContext _context = null!;
        private TaskRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Tasks")
                .Options;
            _context = new TasksDbContext(options);
            _repo = new TaskRepository(_context);
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddTask()
        {
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Test" };
            await _repo.AddAsync(task);
            var fromDb = await _repo.GetByIdAsync(task.Id);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual("Test", fromDb?.Title);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemoveTask()
        {
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "To Delete" };
            await _repo.AddAsync(task);
            await _repo.DeleteAsync(task.Id);
            var fromDb = await _repo.GetByIdAsync(task.Id);
            Assert.IsNull(fromDb);
        }
    }
}
