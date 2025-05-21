using Microsoft.EntityFrameworkCore;
using ToDoListServer.Data;

namespace ToDoListServer.Tests.TestHelpers
{
    public static class InMemoryDbContextFactory
    {
        public static TasksDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new TasksDbContext(options);
        }
    }
}
