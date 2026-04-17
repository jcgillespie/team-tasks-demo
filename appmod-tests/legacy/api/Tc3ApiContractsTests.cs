namespace TeamTasks.Legacy.Behavioral.Api.Tests;

/// <summary>TC-3 — API contracts (black-box HTTP).</summary>
[Collection("legacy-api")]
public sealed class Tc3ApiContractsTests(LegacyApiFixture fx)
{
    private readonly HttpClient _http = fx.Client;

    [Fact(DisplayName = "TC-3.01 — GET success")]
    public async Task TC301_GetList_Returns200JsonArray()
    {
        using var response = await _http.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("json", response.Content.Headers.ContentType?.MediaType ?? "", StringComparison.OrdinalIgnoreCase);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        foreach (var el in doc.RootElement.EnumerateArray())
        {
            Assert.True(el.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.Number);
            Assert.True(el.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String);
            Assert.True(el.TryGetProperty("description", out _));
            Assert.True(el.TryGetProperty("isCompleted", out var done) && done.ValueKind is JsonValueKind.True or JsonValueKind.False);
            Assert.True(el.TryGetProperty("createdAt", out var ca) && ca.ValueKind == JsonValueKind.String);
        }
    }

    [Fact(DisplayName = "TC-3.02 — POST success")]
    public async Task TC302_PostCreate_Returns201WithBody()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc302-{tag}", description = (string?)null },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(body);
        Assert.Equal($"tc302-{tag}", body.Title);
        Assert.True(body.Id > 0);
    }

    [Fact(DisplayName = "TC-3.03 — POST validation failure")]
    public async Task TC303_PostMissingTitle_Returns400WithValidationShape()
    {
        using var response = await _http.PostAsJsonAsync("api/tasks", new { }, JsonOptions.Web);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var raw = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(raw);
        Assert.True(doc.RootElement.TryGetProperty("errors", out _) || doc.RootElement.TryGetProperty("title", out _));
    }

    [Fact(DisplayName = "TC-3.04 — PATCH success")]
    public async Task TC304_PatchToggle_Returns200WithToggledFlag()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var create = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc304-{tag}", description = (string?)null },
            JsonOptions.Web);
        var created = await create.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(created);

        using var patch = await _http.PatchAsync($"api/tasks/{created.Id}/toggle", null);
        patch.EnsureSuccessStatusCode();
        var updated = await patch.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(updated);
        Assert.NotEqual(created.IsCompleted, updated.IsCompleted);
    }

    [Fact(DisplayName = "TC-3.05 — PATCH not found")]
    public async Task TC305_PatchUnknown_Returns404()
    {
        using var response = await _http.PatchAsync("api/tasks/2147483645/toggle", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "TC-3.06 — CORS preflight for GET/POST/PATCH")]
    public async Task TC306_CorsPreflight_AllowsConfiguredOrigin()
    {
        var origin = Environment.GetEnvironmentVariable("TEAM_TASKS_CORS_TEST_ORIGIN")?.Trim()
            ?? "http://127.0.0.1:5173";

        using var getPreflight = await SendOptionsAsync("api/tasks", "GET", origin);
        Assert.Equal(HttpStatusCode.NoContent, getPreflight.StatusCode);
        Assert.Equal(origin, GetAllowOrigin(getPreflight));

        using var postPreflight = await SendOptionsAsync("api/tasks", "POST", origin);
        Assert.Equal(HttpStatusCode.NoContent, postPreflight.StatusCode);
        Assert.Equal(origin, GetAllowOrigin(postPreflight));

        using var patchPreflight = await SendOptionsAsync("api/tasks/1/toggle", "PATCH", origin);
        Assert.Equal(HttpStatusCode.NoContent, patchPreflight.StatusCode);
        Assert.Equal(origin, GetAllowOrigin(patchPreflight));
    }

    [Fact(DisplayName = "TC-3.07 — Location header on 201")]
    public async Task TC307_PostCreate_HasLocationHeader()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/tasks")
        {
            Content = JsonContent.Create(
                new { title = $"tc307-{tag}", description = (string?)null },
                options: JsonOptions.Web),
        };
        using var response = await _http.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Location", out var locs));
        var location = Assert.Single(locs);
        Assert.False(string.IsNullOrWhiteSpace(location));
        Assert.Contains("/api/tasks", location, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<HttpResponseMessage> SendOptionsAsync(string relativeUrl, string accessControlRequestMethod, string origin)
    {
        using var req = new HttpRequestMessage(HttpMethod.Options, relativeUrl);
        req.Headers.TryAddWithoutValidation("Origin", origin);
        req.Headers.TryAddWithoutValidation("Access-Control-Request-Method", accessControlRequestMethod);
        return await _http.SendAsync(req);
    }

    private static string? GetAllowOrigin(HttpResponseMessage response) =>
        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var v) ? Assert.Single(v) : null;
}
