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
        try
        {
            var childExists = await _taskService.CheckChildExistsAsync(childId);
            if (!childExists)
                return NotFound($"Child with ID {childId} does not exist.");

            var activeTasks = await _taskService.GetActiveTasksForChildAsync(childId);
            if (activeTasks == null || activeTasks.Count == 0)
                return NotFound($"No active chores for child with ID {childId}.");

            return Ok(activeTasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    // ✅ Get assigned tasks with validations
    [HttpGet("{childId}/tasks")]
    public async Task<IActionResult> GetTasks(int childId)
    {
        try
        {
            // ✅ Validate if child exists
            var childExists = await _taskService.CheckChildExistsAsync(childId);
            if (!childExists)
                return NotFound($"Child with ID {childId} does not exist.");

            // ✅ Get tasks
            var tasks = await _taskService.GetTasksByChildIdAsync(childId);

            // ✅ Check if tasks found
            if (tasks == null || tasks.Count == 0)
                return NotFound($"No tasks assigned to child with ID {childId}.");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            // ✅ Log exception in real apps
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    // ✅ Mark task as completed with photo upload
    [HttpPost("{childId}/tasks/{taskId}/complete")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CompleteTask(int childId, int taskId, [FromForm] CompleteTaskDto dto)
    {
        if (dto.PhotoFile == null || dto.PhotoFile.Length == 0)
        {
            return BadRequest("Photo is required to complete the task.");
        }

        var result = await _taskService.MarkTaskCompleted(childId, taskId, dto.PhotoFile);
        if (!result)
            return NotFound("Task not found, not assigned to this child, or photo upload failed.");

        return Ok("Task marked as completed with photo.");
    }


    //// ✅ View rewards for this child
    [HttpGet("{childId}/rewards")]
    public async Task<IActionResult> GetRewards(int childId)
    {
        try
        {
            var rewards = await _rewardService.GetRewardsByChildIdAsync(childId);
            return Ok(rewards);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }




}
