namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>TC-5 — Security (see .modernization/output/team-tasks-test-suite.md).</summary>
[Collection("api")]
public sealed class Tc5SecurityTests(ApiFixture fixture)
{
    private readonly HttpClient _http = fixture.Client;

    /// <summary>TC-5.01 — Anonymous GET list: 200 when store available.</summary>
    [Fact]
    public async Task TC_5_01_AnonymousGet_Returns200()
    {
        using var client = TeamTasksApiFactory.CreateClient();
        using var response = await client.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>TC-5.02 — Anonymous mutate: no 401 solely for missing credentials.</summary>
    [Fact]
    public async Task TC_5_02_AnonymousMutate_Not401ForMissingAuth()
    {
        using var client = TeamTasksApiFactory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        using var post = await client.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc502-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        Assert.NotEqual(HttpStatusCode.Unauthorized, post.StatusCode);

        if (post.StatusCode != HttpStatusCode.Created)
            return;

        var created = await post.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(created);

        using var patch = await client.PatchAsync($"api/tasks/{created.Id}/toggle", null);
        Assert.NotEqual(HttpStatusCode.Unauthorized, patch.StatusCode);
    }

    /// <summary>TC-5.03 — Disallowed origin: server omits success CORS allow for that origin.</summary>
    [Fact]
    public async Task TC_5_03_DisallowedOrigin_PreflightDoesNotAllowOrigin()
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, "api/tasks");
        request.Headers.TryAddWithoutValidation("Origin", "https://disallowed.example.test");
        request.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "GET");

        using var response = await _http.SendAsync(request);
        var allowOrigin = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values)
            ? string.Join(",", values)
            : null;

        Assert.True(
            string.IsNullOrEmpty(allowOrigin) || !allowOrigin.Contains("disallowed.example.test", StringComparison.OrdinalIgnoreCase),
            "Disallowed origin should not receive an allow for that host.");
    }

    /// <summary>TC-5.04 — JSON responses do not echo connection strings or keys.</summary>
    [Fact]
    public async Task TC_5_04_ResponsesDoNotContainSecrets()
    {
        using var response = await _http.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("ConnectionStrings", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DefaultConnection", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Password=", text, StringComparison.OrdinalIgnoreCase);
    }
}
