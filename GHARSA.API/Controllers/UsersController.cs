
using System;
using System.Security.Claims;
using System.Threading.Tasks;

using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _users;

        public UsersController(UserService users)
        {
            _users = users;
        }

        // GET api/Users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _users.GetByIdAsync(
                userId.Value
            );

            if (user == null)
                return NotFound();

            return Ok(ToResponse(user));
        }

        // PUT api/Users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateProfileRequest request)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(
                    "Full name is required."
                );
            }

            bool updated = await _users.UpdateProfileAsync(
                userId.Value,
                request.FullName
            );

            if (!updated)
                return NotFound();

            var user = await _users.GetByIdAsync(
                userId.Value
            );

            if (user == null)
                return NotFound();

            return Ok(ToResponse(user));
        }

        // GET CURRENT USER ID FROM JWT
        private int? GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (int.TryParse(id, out int userId))
                return userId;

            return null;
        }

        // CONVERT USER TO RESPONSE DTO
        private static UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = user.Role
            };
        }
    }
}
