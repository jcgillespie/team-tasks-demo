namespace TeamTasks.Legacy.Behavioral.Api.Tests;

/// <summary>TC-5 — Security (black-box HTTP).</summary>
[Collection("legacy-api")]
public sealed class Tc5SecurityTests(LegacyApiFixture fx)
{
    private readonly HttpClient _http = fx.Client;

    [Fact(DisplayName = "TC-5.01 — Anonymous list")]
    public async Task TC501_GetWithoutCredentials_Returns200()
    {
        using var client = new HttpClient { BaseAddress = _http.BaseAddress, Timeout = _http.Timeout };
        using var response = await client.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "TC-5.02 — Anonymous mutate (no 401 for missing auth)")]
    public async Task TC502_MutateWithoutCredentials_Not401()
    {
        using var client = new HttpClient { BaseAddress = _http.BaseAddress, Timeout = _http.Timeout };
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var post = await client.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc502-{tag}", description = (string?)null },
            JsonOptions.Web);
        Assert.NotEqual(HttpStatusCode.Unauthorized, post.StatusCode);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var body = await post.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(body);

        using var patch = await client.PatchAsync($"api/tasks/{body.Id}/toggle", null);
        Assert.NotEqual(HttpStatusCode.Unauthorized, patch.StatusCode);
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }

    [Fact(DisplayName = "TC-5.03 — Disallowed origin: no Access-Control-Allow-Origin for evil origin")]
    public async Task TC503_DisallowedOrigin_NoCorsAllowHeader()
    {
        using var req = new HttpRequestMessage(HttpMethod.Options, "api/tasks");
        req.Headers.TryAddWithoutValidation("Origin", "https://evil.example");
        req.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "GET");
        using var response = await _http.SendAsync(req);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact(DisplayName = "TC-5.04 — No secrets in JSON")]
    public async Task TC504_ResponsesDoNotEchoSecrets()
    {
        using var response = await _http.GetAsync("api/tasks");
        response.EnsureSuccessStatusCode();
        var raw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("connectionstring", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("connectionstrings", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password=", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Data Source=", raw, StringComparison.OrdinalIgnoreCase);
    }
}
