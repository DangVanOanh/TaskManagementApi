using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services;

namespace TaskManagement.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        // An in-memory SQLite database lives only while its connection stays open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _service = new TaskService(_db);
    }

    [Fact]
    public async Task CreateAsync_ValidTask_AssignsIdAndPersists()
    {
        var task = new TaskItem { Title = "Write report", Status = TaskItemStatus.InProgress };

        var created = await _service.CreateAsync(task);

        Assert.True(created.Id > 0);
        var loaded = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(loaded);
        Assert.Equal("Write report", loaded.Title);
        Assert.Equal(TaskItemStatus.InProgress, loaded.Status);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
