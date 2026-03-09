using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Tasks.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Set up backend API",
                Description = "Create task endpoints and wire EF Core.",
                IsCompleted = true,
                CreatedAt = now.AddMinutes(-30)
            },
            new TaskItem
            {
                Title = "Build frontend task page",
                Description = "Create list and form UI in React.",
                IsCompleted = false,
                CreatedAt = now.AddMinutes(-20)
            },
            new TaskItem
            {
                Title = "Write baseline tests",
                Description = "Cover API and frontend basics.",
                IsCompleted = false,
                CreatedAt = now.AddMinutes(-10)
            });

        await context.SaveChangesAsync();
    }
}
