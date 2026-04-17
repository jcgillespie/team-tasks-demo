namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>TC-3 — API contracts (see .modernization/output/team-tasks-test-suite.md).</summary>
[Collection("api")]
public sealed class Tc3ApiContractsTests(ApiFixture fixture)
{
    private readonly HttpClient _http = fixture.Client;

    /// <summary>TC-3.01 — GET success: 200 + JSON array with required shape.</summary>
    [Fact]
    public async Task TC_3_01_GetList_Returns200JsonArray()
    {
        using var response = await _http.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var list = await response.Content.ReadFromJsonAsync<List<TaskResponse>>(JsonOptions.CamelCase);
        Assert.NotNull(list);
        foreach (var item in list)
        {
            Assert.True(item.Id > 0);
            Assert.NotNull(item.Title);
            _ = item.Description;
            _ = item.IsCompleted;
            _ = item.CreatedAt;
        }
    }

    /// <summary>TC-3.02 — POST success: 201 + body.</summary>
    [Fact]
    public async Task TC_3_02_PostCreate_Returns201WithBody()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc302-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(body);
        Assert.Equal($"tc302-{suffix}", body.Title);
    }

    /// <summary>TC-3.03 — POST validation failure: 400 + structured validation payload.</summary>
    [Fact]
    public async Task TC_3_03_PostValidationFailure_Returns400WithStructuredBody()
    {
        using var emptyObject = await _http.PostAsync(
            "api/tasks",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, emptyObject.StatusCode);
        var text1 = await emptyObject.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(text1));
        Assert.StartsWith(
            "application/",
            emptyObject.Content.Headers.ContentType?.MediaType ?? string.Empty,
            StringComparison.Ordinal);

        using var missingTitle = await _http.PostAsJsonAsync(
            "api/tasks",
            new { description = "only" },
            JsonOptions.CamelCase);
        Assert.Equal(HttpStatusCode.BadRequest, missingTitle.StatusCode);
        var text2 = await missingTitle.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(text2));
    }

    /// <summary>TC-3.04 — PATCH success: 200 + toggled isCompleted.</summary>
    [Fact]
    public async Task TC_3_04_PatchToggle_Returns200WithUpdatedTask()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var created = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc304-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        var task = await created.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(task);

        using var patch = await _http.PatchAsync($"api/tasks/{task.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        var updated = await patch.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(updated);
        Assert.NotEqual(task.IsCompleted, updated.IsCompleted);
    }

    /// <summary>TC-3.05 — PATCH not found: 404.</summary>
    [Fact]
    public async Task TC_3_05_PatchToggleMissing_Returns404()
    {
        using var listResponse = await _http.GetAsync("api/tasks");
        var list = await listResponse.Content.ReadFromJsonAsync<List<TaskResponse>>(JsonOptions.CamelCase);
        Assert.NotNull(list);
        var maxId = list.Count == 0 ? 0 : list.Max(t => t.Id);
        var missingId = maxId + 999_999;

        using var patch = await _http.PatchAsync($"api/tasks/{missingId}/toggle", null);
        Assert.Equal(HttpStatusCode.NotFound, patch.StatusCode);
    }

    /// <summary>TC-3.06 — CORS preflight: allowed origin receives CORS headers for subsequent methods.</summary>
    [Fact]
    public async Task TC_3_06_CorsPreflight_AllowedOrigin_GetsCorsHeaders()
    {
        var origin = Environment.GetEnvironmentVariable("TEAM_TASKS_CORS_ALLOWED_ORIGIN") ?? "http://localhost:5173";
        using var request = new HttpRequestMessage(HttpMethod.Options, "api/tasks");
        request.Headers.TryAddWithoutValidation("Origin", origin);
        request.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "GET");

        using var response = await _http.SendAsync(request);
        Assert.True(
            (int)response.StatusCode is >= 200 and < 300,
            $"OPTIONS expected success; got {(int)response.StatusCode}.");

        var allowOrigin = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            ? string.Join(",", origins)
            : null;
        Assert.False(string.IsNullOrEmpty(allowOrigin));
    }

    /// <summary>TC-3.07 — Location header present on 201 (exact URL not asserted per spec).</summary>
    [Fact]
    public async Task TC_3_07_PostCreate_HasLocationHeader()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc307-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(
            response.Headers.Location is not null,
            "201 create response should include a Location header per spec TC-3.07 (URL shape not asserted).");
    }
}
