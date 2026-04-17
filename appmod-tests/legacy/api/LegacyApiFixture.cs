namespace TeamTasks.Legacy.Behavioral.Api.Tests;

/// <summary>
/// Black-box HTTP client for the legacy API. Base URL from TEAM_TASKS_API_BASE_URL (default http://127.0.0.1:5276).
/// </summary>
public sealed class LegacyApiFixture : IDisposable
{
    public HttpClient Client { get; }

    public LegacyApiFixture()
    {
        var baseUrl = Environment.GetEnvironmentVariable("TEAM_TASKS_API_BASE_URL")?.TrimEnd('/')
            ?? "http://127.0.0.1:5276";
        Client = new HttpClient { BaseAddress = new Uri(baseUrl + "/"), Timeout = TimeSpan.FromSeconds(60) };
    }

    public void Dispose() => Client.Dispose();
}

[CollectionDefinition("legacy-api")]
public sealed class LegacyApiCollection : ICollectionFixture<LegacyApiFixture>;
