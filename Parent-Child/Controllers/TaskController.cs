using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Models;

[ApiController]
[Route("api/tasks")]
[Authorize(Policy = "ParentPolicy")]
public class ParentDashboardController : ControllerBase
{
    private readonly ITaskService _service;
    public ParentDashboardController(ITaskService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _service.GetAllAsync();
        if (tasks == null || !tasks.Any())
            return NotFound("No tasks found.");
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                UserId = dto.UserId,
                AssignedToId = dto.AssignedToId,
                RewardId = dto.RewardId,
                IsCompleted = dto.IsCompleted,
                IsApproved = dto.IsApproved
            };

            var result = await _service.CreateAsync(task);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var result = await _service.ApproveTask(id);
            return Ok("Task approved successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var result = await _service.RejectTask(id);
            return Ok("Task marked for redo.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
