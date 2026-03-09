using Microsoft.AspNetCore.Mvc;
using TeamTasks.Api.Contracts;
using TeamTasks.Api.Services;

namespace TeamTasks.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> GetTasks(CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetTasksAsync(cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var created = await taskService.CreateTaskAsync(request.Title, request.Description, cancellationToken);

        return CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/toggle")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> ToggleTask(int id, CancellationToken cancellationToken)
    {
        var updated = await taskService.ToggleTaskAsync(id, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
