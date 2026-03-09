using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.Contracts;

public class CreateTaskRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}
