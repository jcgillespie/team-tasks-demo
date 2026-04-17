namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>
/// Spec: TC-3.01 shape — id, title, description, isCompleted, createdAt.
/// </summary>
internal sealed record TaskResponse(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset CreatedAt);
