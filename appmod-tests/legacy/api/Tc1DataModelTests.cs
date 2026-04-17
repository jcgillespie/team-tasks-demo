namespace TeamTasks.Legacy.Behavioral.Api.Tests;

/// <summary>TC-1 — Data model &amp; persistence (black-box HTTP).</summary>
[Collection("legacy-api")]
public sealed class Tc1DataModelTests(LegacyApiFixture fx)
{
    private readonly HttpClient _http = fx.Client;

    [Fact(DisplayName = "TC-1.01 — Title over max length → 400, no insert")]
    public async Task TC101_TitleOverMaxLength_Returns400()
    {
        var title = new string('a', 121);
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title, description = "valid" },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "TC-1.02 — Description over max length → 400")]
    public async Task TC102_DescriptionOverMaxLength_Returns400()
    {
        var description = new string('b', 1001);
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "ok", description },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "TC-1.03 — Trim persistence")]
    public async Task TC103_TrimPersistence_StoredValuesTrimmed()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "  hello  ", description = "  x  " },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(body);
        Assert.Equal("hello", body.Title);
        Assert.Equal("x", body.Description);
    }

    [Fact(DisplayName = "TC-1.04 — Seed idempotency (requires manual restart — see README)", Skip = "TC-1.04 requires stopping and restarting the API against the same database file; automated HTTP tests cannot observe a second process start. Follow README manual steps.")]
    public void TC104_SeedIdempotency_ManualOnly()
    {
    }

    [Fact(DisplayName = "TC-1.05 — Distinct identifiers")]
    public async Task TC105_TwoCreates_DistinctPositiveIds()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var r1 = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc105-a-{tag}", description = (string?)null },
            JsonOptions.Web);
        using var r2 = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"tc105-b-{tag}", description = (string?)null },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, r2.StatusCode);
        var t1 = await r1.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        var t2 = await r2.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(t1);
        Assert.NotNull(t2);
        Assert.True(t1.Id > 0);
        Assert.True(t2.Id > 0);
        Assert.NotEqual(t1.Id, t2.Id);
    }
}
