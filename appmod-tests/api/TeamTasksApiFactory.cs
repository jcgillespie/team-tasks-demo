namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>
/// Resolves base URL from <c>TEAM_TASKS_API_BASE_URL</c> (no trailing slash required).
/// </summary>
internal static class TeamTasksApiFactory
{
    internal static HttpClient CreateClient()
    {
        var raw = Environment.GetEnvironmentVariable("TEAM_TASKS_API_BASE_URL");
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException(
                "Set environment variable TEAM_TASKS_API_BASE_URL to the legacy API root (e.g. http://localhost:5276).");

        var baseUri = raw.TrimEnd('/');
        var client = new HttpClient { BaseAddress = new Uri(baseUri + "/", UriKind.Absolute) };
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        return client;
    }
}
