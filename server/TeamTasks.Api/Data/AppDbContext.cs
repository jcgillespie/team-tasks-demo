using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Data;

public enum DatabaseProviderMode
{
    Sqlite,
    SqlServer
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public static void ConfigureProvider(DbContextOptionsBuilder optionsBuilder, string connectionString, DatabaseProviderMode providerMode)
    {
        if (providerMode == DatabaseProviderMode.SqlServer)
        {
            optionsBuilder.UseSqlServer(connectionString);
            return;
        }

        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(120);
            entity.Property(t => t.Description).HasMaxLength(1000);
            entity.Property(t => t.CreatedAt).IsRequired();
        });
    }
}
