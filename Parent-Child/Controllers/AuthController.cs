using Microsoft.AspNetCore.Mvc;
using Parent_Child.Services;     // ✅ This is the missing using
using Parent_Child.Models;       // ✅ Needed to recognize 'User'
using Parent_Child.DTOs;         // ✅ Needed to recognize 'LoginDto'

namespace Parent_Child.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = dto.Password,
                    Role = dto.Role,
                    IsGoogleUser = dto.IsGoogleUser
                };

                var created = await _authService.RegisterAsync(user);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                // For missing required fields
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // For duplicate email or other general errors
                return Conflict(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials." });

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                // For missing email or password input
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // For unexpected errors
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }


}
