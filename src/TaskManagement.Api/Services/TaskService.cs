using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _db.TaskItems
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _db.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        // The database generates the Id, so ignore any value sent by the client.
        task.Id = 0;

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(int id, TaskItem task)
    {
        var existing = await _db.TaskItems.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        // The Id comes from the route, not from the request body.
        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.DueDate = task.DueDate;
        existing.AssigneeName = task.AssigneeName;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.TaskItems.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _db.TaskItems.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
