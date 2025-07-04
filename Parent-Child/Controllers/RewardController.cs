using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Models;
[ApiController]
[Route("api/rewards")]
public class RewardController : ControllerBase
{
    private readonly IRewardService _service;
    public RewardController(IRewardService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rewards = await _service.GetAllAsync();
        if (rewards == null || !rewards.Any())
            return NotFound("No rewards found.");
        return Ok(rewards);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRewardDto dto)
    {

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var reward = new Reward
            {
                Title = dto.Title,
                Description = dto.Description
            };

            var created = await _service.CreateAsync(reward);
            return Ok(created);
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




    //[HttpGet("child/{childId}")]
    //public async Task<IActionResult> GetRewardsByChildId(int childId)
    //{
    //    if (childId <= 0)
    //        return BadRequest("Invalid child ID.");

    //    try
    //    {
    //        var rewards = await _service.GetRewardsByChildIdAsync(childId);
    //        return Ok(rewards);
    //    }
    //    catch (Exception ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //}


    //[HttpPut("redeem/{taskId}")]
    //public async Task<IActionResult> RedeemReward(int taskId)
    //{
    //    if (taskId <= 0)
    //        return BadRequest("Invalid task ID.");

    //    try
    //    {
    //        var result = await _service.RedeemRewardAsync(taskId);
    //        if (result == null)
    //            return NotFound($"Task with ID {taskId} not found or already redeemed.");

    //        return Ok($"Reward for task ID {taskId} redeemed successfully.");
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

}
