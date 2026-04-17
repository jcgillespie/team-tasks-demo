namespace TeamTasks.Behavioral.Api.Tests;

internal static class JsonOptions
{
    internal static readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
