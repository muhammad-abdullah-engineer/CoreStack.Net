using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services;
using Project.Application.ViewModels.Common;
using Project.Application.ViewModels.Tasks;
using TaskStatus = Project.Domain.Entities.Enums.TaskStatus;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID claim not found");
        }
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid User ID format");
        }
        return userId;
    }

    /// <summary>
    /// Get all tasks for current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskResponseViewModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskResponseViewModel>>>> GetTasks()
    {
        var tasks = await _taskService.GetUserTasksAsync(GetUserId());
        return Ok(ApiResponse<IEnumerable>.SuccessResponse(tasks));
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseViewModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<TaskResponseViewModel>>> GetTask(Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id, GetUserId());
            return Ok(ApiResponse<TaskResponseViewModel>.SuccessResponse(task, "Task retrieved successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<TaskResponseViewModel>.ErrorResponse("Task not found"));
        }
    }

    /// <summary>
    /// Get tasks by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskResponseViewModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskResponseViewModel>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskResponseViewModel>>>> GetTasksByStatus(int status)
    {
        if (!Enum.IsDefined(typeof(TaskStatus), status))
        {
            return BadRequest(ApiResponse<IEnumerable>.ErrorResponse("Invalid status"));
        }

        var tasks = await _taskService.GetTasksByStatusAsync(GetUserId(), (TaskStatus)status);
        return Ok(ApiResponse<IEnumerable>.SuccessResponse(tasks));
    }

    /// <summary>
    /// Get task statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatistics()
    {
        var stats = await _taskService.GetTaskStatisticsAsync(GetUserId());
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }

    /// <summary>
    /// Create new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> CreateTask(CreateTaskViewModel model)
    {
        var taskId = await _taskService.CreateTaskAsync(GetUserId(), model);

        return CreatedAtAction(
            nameof(GetTask),
            new { id = taskId },
            ApiResponse<object>.SuccessResponse(new { Id = taskId }, "Task created successfully"));
    }

    /// <summary>
    /// Update task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTask(Guid id, UpdateTaskViewModel model)
    {
        try
        {
            await _taskService.UpdateTaskAsync(id, GetUserId(), model);

            // Return success with no content (or you can fetch the updated task if needed)
            return Ok(ApiResponse<object>.SuccessResponse(true, "Task updated successfully"));
        }
        catch (KeyNotFoundException)
        {
            // Return not found response
            return NotFound(ApiResponse<object>.ErrorResponse("Task not found"));
        }
    }

    /// <summary>
    /// Delete task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(Guid id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id, GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(true, "Task deleted successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Task not found"));
        }
    }
}
