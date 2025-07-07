using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parent_Child.Services;

[ApiController]
[Authorize(Policy = "ChildOrParentPolicy")]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _service;
    public StatsController(IStatsService service)
    {
        _service = service;
    }


    [HttpGet("{childId}")]
    public async Task<IActionResult> GetChildStats(int childId)
    {
        try
        {
            var stats = await _service.GetChildStatsAsync(childId);
            return Ok(stats);
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
