using GHARSA.API.DTOs;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _users;
        private readonly JwtTokenService _tokens;

        public AuthController(
            UserService users,
            JwtTokenService tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return BadRequest("Full name is required.");

            if (request.Password.Length < 8)
                return BadRequest(
                    "Password must have at least 8 characters.");

            try
            {
                var user = await _users.RegisterAsync(
                    request.FullName,
                    request.Email,
                    request.Password);

                if (user == null)
                    return Conflict(
                        "An account with this email already exists.");

                var response = _tokens.CreateToken(user);

                return StatusCode(201, response);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to register this email.");
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var user = await _users.AuthenticateAsync(
                request.Email,
                request.Password);

            if (user == null)
                return Unauthorized(
                    "Invalid email or password.");

            return Ok(_tokens.CreateToken(user));
        }
    }
}