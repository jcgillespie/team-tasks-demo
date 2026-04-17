namespace TeamTasks.Legacy.Behavioral.Api.Tests;

/// <summary>TC-2 — Business logic (black-box HTTP).</summary>
[Collection("legacy-api")]
public sealed class Tc2BusinessLogicTests(LegacyApiFixture fx)
{
    private readonly HttpClient _http = fx.Client;

    [Fact(DisplayName = "TC-2.01 — List order newest first")]
    public async Task TC201_GetTasks_OrderedByCreatedAtDescending()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var older = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"order-first-{tag}", description = (string?)null },
            JsonOptions.Web);
        Assert.Equal(HttpStatusCode.Created, older.StatusCode);
        await Task.Delay(50);
        using var newer = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"order-second-{tag}", description = (string?)null },
            JsonOptions.Web);
        Assert.Equal(HttpStatusCode.Created, newer.StatusCode);
        var newerBody = await newer.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(newerBody);

        using var list = await _http.GetAsync("api/tasks");
        list.EnsureSuccessStatusCode();
        var tasks = await list.Content.ReadFromJsonAsync<List<TaskResponse>>(JsonOptions.Web);
        Assert.NotNull(tasks);

        var idxNewer = tasks.FindIndex(t => t.Title == $"order-second-{tag}");
        var idxOlder = tasks.FindIndex(t => t.Title == $"order-first-{tag}");
        Assert.True(idxNewer >= 0);
        Assert.True(idxOlder >= 0);
        Assert.True(idxNewer < idxOlder, "Newer task must appear before older (descending CreatedAt).");
    }

    [Fact(DisplayName = "TC-2.02 — Double toggle")]
    public async Task TC202_ToggleTwice_FlipsCompletionBack()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        using var create = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"toggle-{tag}", description = (string?)null },
            JsonOptions.Web);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(created);
        Assert.False(created.IsCompleted);

        using var first = await _http.PatchAsync($"api/tasks/{created.Id}/toggle", null);
        first.EnsureSuccessStatusCode();
        var afterFirst = await first.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(afterFirst);
        Assert.True(afterFirst.IsCompleted);

        using var second = await _http.PatchAsync($"api/tasks/{created.Id}/toggle", null);
        second.EnsureSuccessStatusCode();
        var afterSecond = await second.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.Web);
        Assert.NotNull(afterSecond);
        Assert.False(afterSecond.IsCompleted);
    }

    [Fact(DisplayName = "TC-2.03 — Toggle unknown id → 404")]
    public async Task TC203_ToggleUnknownId_Returns404()
    {
        using var response = await _http.PatchAsync("api/tasks/2147483646/toggle", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "TC-2.04 — Whitespace-only title (API) — live behavior: 400 validation")]
    public async Task TC204_WhitespaceOnlyTitle_Returns400()
    {
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "   ", description = (string?)null },
            JsonOptions.Web);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var raw = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title", raw, StringComparison.OrdinalIgnoreCase);
    }
}
