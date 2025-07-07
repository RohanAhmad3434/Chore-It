using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Parent_Child.DTOs;         // ✅ Needed to recognize 'LoginDto'
using Parent_Child.Models;       // ✅ Needed to recognize 'User'
using Parent_Child.Services;     // ✅ This is the missing using

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
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var (accessToken, refreshToken, user) = await _authService.LoginWithTokensAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                    return Unauthorized("Invalid credentials");

                return Ok(new { accessToken, refreshToken, user });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Invalid client request");

            try
            {
                var (newAccessToken, newRefreshToken) = await _authService.RefreshTokenAsync(tokenApiDto.AccessToken, tokenApiDto.RefreshToken);
                return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }


}
