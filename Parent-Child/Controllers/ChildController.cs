using Microsoft.AspNetCore.Mvc;

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

    // ✅ Mark task as completed
    [HttpPost("{childId}/tasks/{taskId}/complete")]
    public async Task<IActionResult> CompleteTask(int childId, int taskId)
    {
        var result = await _taskService.MarkTaskCompleted(childId, taskId);
        if (!result) return NotFound("Task not found or not assigned to this child");
        return Ok("Task marked as completed");
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
