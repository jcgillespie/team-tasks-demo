namespace TeamTasks.Legacy.Behavioral.Api.Tests;

internal sealed class TaskResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
