using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;

[ApiController]
[Route("api/rewards")]
public class RewardController : ControllerBase
{
    private readonly IRewardService _service;
    public RewardController(IRewardService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRewardDto dto)
    {
        var reward = new Reward
        {
            Title = dto.Title,
            Description = dto.Description
        };

        var created = await _service.CreateAsync(reward);
        return Ok(created);
    }
}
