namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>Shared HTTP client for behavioral API tests (one per collection).</summary>
public sealed class ApiFixture : IDisposable
{
    public HttpClient Client { get; }

    public ApiFixture()
    {
        Client = TeamTasksApiFactory.CreateClient();
    }

    public void Dispose() => Client.Dispose();
}

[CollectionDefinition("api")]
public sealed class ApiTestCollection : ICollectionFixture<ApiFixture>
{
}
