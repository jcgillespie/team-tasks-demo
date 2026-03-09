using TeamTasks.Api.Contracts;

namespace TeamTasks.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyCollection<TaskResponse>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task<TaskResponse> CreateTaskAsync(string title, string? description, CancellationToken cancellationToken = default);

    Task<TaskResponse?> ToggleTaskAsync(int id, CancellationToken cancellationToken = default);
}
