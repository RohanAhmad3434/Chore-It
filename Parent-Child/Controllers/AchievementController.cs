using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Models;
using Parent_Child.Services;

namespace Parent_Child.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _service;

        public AchievementController(IAchievementService service)
        {
            _service = service;
        }

        /// ✅ Create a new achievement with icon upload
        [HttpPost("create")]
        public async Task<IActionResult> CreateAchievement([FromForm] AchievementCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAchievementAsync(dto, Request);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// ✅ Get all achievements (admin panel)
      
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var achievements = await _service.GetAllAchievementsAsync();
            return Ok(achievements);
        }

        /// ✅ Get achievements earned by a specific child
        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetChildAchievements(int childId)
        {
            try
            {
                var achievements = await _service.GetChildAchievementsAsync(childId);
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



        /// ✅ Update an achievement
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAchievement(int id, [FromForm] AchievementUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAchievementAsync(id, dto, Request);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// ✅ Delete an achievement
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            try
            {
                var result = await _service.DeleteAchievementAsync(id);
                return Ok(new { message = $"Achievement {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


    }
}
