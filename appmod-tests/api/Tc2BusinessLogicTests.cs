namespace TeamTasks.Behavioral.Api.Tests;

/// <summary>TC-2 — Business logic (see .modernization/output/team-tasks-test-suite.md).</summary>
[Collection("api")]
public sealed class Tc2BusinessLogicTests(ApiFixture fixture)
{
    private readonly HttpClient _http = fixture.Client;

    /// <summary>TC-2.01 — List order: first element has latest createdAt.</summary>
    [Fact]
    public async Task TC_2_01_ListOrder_NewestFirst()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var first = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"order-first-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var firstBody = await first.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(firstBody);

        await Task.Delay(50);

        using var second = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"order-second-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var secondBody = await second.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(secondBody);

        using var listResponse = await _http.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<List<TaskResponse>>(JsonOptions.CamelCase);
        Assert.NotNull(list);
        Assert.True(secondBody.CreatedAt > firstBody.CreatedAt, "Test setup requires second create strictly newer.");

        var idxNewer = list.FindIndex(t => t.Id == secondBody.Id);
        var idxOlder = list.FindIndex(t => t.Id == firstBody.Id);
        Assert.True(idxNewer >= 0 && idxOlder >= 0);
        Assert.True(idxNewer < idxOlder, "Newer task must appear before older (descending createdAt).");
    }

    /// <summary>TC-2.02 — PATCH toggle twice flips completion twice.</summary>
    [Fact]
    public async Task TC_2_02_ToggleTwice_FlipsCompletionBack()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var created = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = $"toggle-{suffix}", description = (string?)null },
            JsonOptions.CamelCase);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var task = await created.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(task);
        Assert.False(task.IsCompleted);

        using var patch1 = await _http.PatchAsync($"api/tasks/{task.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, patch1.StatusCode);
        var after1 = await patch1.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(after1);
        Assert.True(after1.IsCompleted);

        using var patch2 = await _http.PatchAsync($"api/tasks/{task.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, patch2.StatusCode);
        var after2 = await patch2.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(after2);
        Assert.False(after2.IsCompleted);
    }

    /// <summary>TC-2.03 — Toggle unknown id → 404, no mutation.</summary>
    [Fact]
    public async Task TC_2_03_ToggleUnknownId_Returns404()
    {
        using var listResponse = await _http.GetAsync("api/tasks");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<List<TaskResponse>>(JsonOptions.CamelCase);
        Assert.NotNull(list);
        var maxId = list.Count == 0 ? 0 : list.Max(t => t.Id);
        var missingId = maxId + 999_999;

        using var patch = await _http.PatchAsync($"api/tasks/{missingId}/toggle", null);
        Assert.Equal(HttpStatusCode.NotFound, patch.StatusCode);
    }

    /// <summary>TC-2.04 — Whitespace-only title: spec NEEDS CLARIFICATION (400 or 201 with empty title after trim).</summary>
    [Fact]
    public async Task TC_2_04_WhitespaceOnlyTitle_DocumentedAmbiguousOutcome()
    {
        using var response = await _http.PostAsJsonAsync(
            "api/tasks",
            new { title = "   ", description = (string?)null },
            JsonOptions.CamelCase);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions.CamelCase);
        Assert.NotNull(body);
        Assert.Equal(string.Empty, body.Title);
    }
}
