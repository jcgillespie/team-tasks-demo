namespace TeamTasks.Api.Contracts;

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAt);
