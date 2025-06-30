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

        //[HttpPost("register")]
        //public async Task<IActionResult> Register(User user)
        //{
        //    var created = await _authService.RegisterAsync(user);
        //    return Ok(created);
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = dto.Password, // Will be hashed inside service
                Role = dto.Role,
                IsGoogleUser = dto.IsGoogleUser
            };

            var created = await _authService.RegisterAsync(user);
            return Ok(created);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            if (user == null) return Unauthorized("Invalid credentials");
            return Ok(user);
        }
    }
}
