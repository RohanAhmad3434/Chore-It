using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;

[ApiController]
[Route("api/child")]
public class ChildDashboardController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IRewardService _rewardService;
    public ChildDashboardController(ITaskService taskService, IRewardService rewardService)
    {
        _taskService = taskService;
        _rewardService = rewardService;
    }


    // ✅ Get active chores for the child
    [HttpGet("{childId}/active-chores")]
    public async Task<IActionResult> GetActiveChores(int childId)
    {
        if (childId <= 0)
            return BadRequest("Invalid child ID.");

        try
        {
            var activeTasks = await _taskService.GetActiveTasksForChildAsync(childId);
            if (activeTasks == null || activeTasks.Count == 0)
                return NotFound($"No active chores for child with ID {childId}.");

            return Ok(activeTasks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    // ✅ Get assigned tasks with validations
    [HttpGet("{childId}/tasks")]
    public async Task<IActionResult> GetTasks(int childId)
    {
        if (childId <= 0)
            return BadRequest("Invalid child ID.");

        try
        {
            var tasks = await _taskService.GetTasksByChildIdAsync(childId);
            if (tasks == null || tasks.Count == 0)
                return NotFound($"No tasks assigned to child with ID {childId}.");

            return Ok(tasks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    // ✅ Mark task as completed with photo upload
    [HttpPost("{childId}/tasks/{taskId}/complete")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CompleteTask(int childId, int taskId, [FromForm] CompleteTaskDto dto)
    {
        if (childId <= 0 || taskId <= 0)
            return BadRequest("Invalid child ID or task ID.");

        if (dto.PhotoFile == null || dto.PhotoFile.Length == 0)
            return BadRequest("Photo is required to complete the task.");

        try
        {
            var result = await _taskService.MarkTaskCompleted(childId, taskId, dto.PhotoFile);
            if (!result)
                return NotFound("Task not found, not assigned to this child, or photo upload failed.");

            return Ok("Task marked as completed with photo.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ✅ View rewards for this child
    [HttpGet("{childId}/rewards")]
    public async Task<IActionResult> GetRewards(int childId)
    {
        if (childId <= 0)
            return BadRequest("Invalid child ID.");

        try
        {
            var rewards = await _rewardService.GetRewardsByChildIdAsync(childId);
            return Ok(rewards);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}
