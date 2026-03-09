using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Contracts;
using TeamTasks.Api.Data;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Services;

public class TaskService(AppDbContext dbContext) : ITaskService
{
    public async Task<IReadOnlyCollection<TaskResponse>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await dbContext.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => ToResponse(t))
            .ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<TaskResponse> CreateTaskAsync(string title, string? description, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(task);
    }

    public async Task<TaskResponse?> ToggleTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.IsCompleted = !task.IsCompleted;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(task);
    }

    private static TaskResponse ToResponse(TaskItem task)
    {
        return new TaskResponse(task.Id, task.Title, task.Description, task.IsCompleted, task.CreatedAt);
    }
}
