using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Models;

[ApiController]
[Route("api/tasks")]
public class ParentDashboardController : ControllerBase
{
    private readonly ITaskService _service;
    public ParentDashboardController(ITaskService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
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

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _service.ApproveTask(id);
        return result ? Ok("Task approved") : NotFound();
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _service.RejectTask(id);
        return result ? Ok("Task marked for redo") : NotFound();
    }
}
