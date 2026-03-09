using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Contracts;
using TeamTasks.Api.Data;
using TeamTasks.Api.Models;
using TeamTasks.Api.Services;

namespace TeamTasks.Api.Tests;

public class TaskServiceTests
{
    [Fact]
    public void CreateTaskRequest_Title_IsRequired()
    {
        var request = new CreateTaskRequest
        {
            Title = string.Empty,
            Description = "test"
        };

        var validationResults = Validate(request);

        Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(CreateTaskRequest.Title)));
    }

    [Fact]
    public async Task GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending()
    {
        await using var context = BuildContext();
        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Older",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
            new TaskItem
            {
                Title = "Newer",
                CreatedAt = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var service = new TaskService(context);

        var tasks = await service.GetTasksAsync();

        Assert.Equal(2, tasks.Count);
        Assert.Equal("Newer", tasks.First().Title);
        Assert.Equal("Older", tasks.Last().Title);
    }

    [Fact]
    public async Task ToggleTaskAsync_FlipsCompletionState()
    {
        await using var context = BuildContext();
        var task = new TaskItem
        {
            Title = "Toggle me",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TaskService(context);

        var firstToggle = await service.ToggleTaskAsync(task.Id);
        var secondToggle = await service.ToggleTaskAsync(task.Id);

        Assert.NotNull(firstToggle);
        Assert.True(firstToggle!.IsCompleted);
        Assert.NotNull(secondToggle);
        Assert.False(secondToggle!.IsCompleted);
    }

    private static List<ValidationResult> Validate(object target)
    {
        var context = new ValidationContext(target);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(target, context, results, validateAllProperties: true);
        return results;
    }

    private static AppDbContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"teamtasks-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}
