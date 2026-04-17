namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>TC-1 — Data model & persistence (see .modernization/output/team-tasks-test-suite.md).</summary>
[Collection("api")]
public sealed class Tc1DataModelTests(ApiFixture fixture)
{
    private readonly HttpClient _http = fixture.Client;

    /// <summary>TC-1.01 — Title over max length → 400, no insert.</summary>
    [Fact]
    public async Task TC_1_01_TitleOverMaxLength_Returns400()
    {
        var title = new string('a', 121);
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title, description = "ok" },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>TC-1.02 — Description over max length → 400.</summary>
    [Fact]
    public async Task TC_1_02_DescriptionOverMaxLength_Returns400()
    {
        var description = new string('b', 1001);
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "ok", description },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>TC-1.03 — Trim persistence: stored title/description match trimmed values.</summary>
    [Fact]
    public async Task TC_1_03_TrimPersistence_Returns201WithTrimmedFields()
    {
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "  hello  ", description = "  x  " },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(body);
        Assert.Equal("hello", body.Title);
        Assert.Equal("x", body.Description);
    }

    /// <summary>TC-1.04 — Seed idempotency (requires two process starts; not automated here).</summary>
    [Fact(Skip = "TC-1.04 requires two application starts against an isolated database; see appmod-tests/README.md")]
    public Task TC_1_04_SeedIdempotency_NotAutomatedInThisSuite() => Task.CompletedTask;

    /// <summary>TC-1.05 — Two successful creates yield distinct positive integer ids.</summary>
    [Fact]
    public async Task TC_1_05_TwoCreates_YieldDistinctPositiveIds()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var r1 = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc105-a-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        using var r2 = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc105-b-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);

        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, r2.StatusCode);
        var t1 = await r1.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        var t2 = await r2.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(t1);
        Assert.NotNull(t2);
        Assert.True(t1.Id > 0);
        Assert.True(t2.Id > 0);
        Assert.NotEqual(t1.Id, t2.Id);
    }
}
