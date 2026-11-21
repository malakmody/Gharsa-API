using System.Security.Claims;

using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CareLogsController : ControllerBase
    {
        private readonly CareLogService _service;

        public CareLogsController(CareLogService service)
        {
            _service = service;
        }

        // GET CURRENT USER ID FROM JWT
        private int? CurrentUserId()
        {
            var value = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(value, out var id)
                ? id
                : null;
        }

        // CONVERT MODEL TO RESPONSE DTO
        private static CareLogResponse ToResponse(CareLog log)
        {
            return new CareLogResponse
            {
                CareLogId = log.CareLogId,
                MyPlantId = log.MyPlantId,
                PlantName = log.MyPlant?.Plant?.Name ?? "",
                TaskName = log.TaskName,
                CompletedAt = log.CompletedAt,
                Notes = log.Notes
            };
        }

        // GET: api/CareLogs
        // GET ALL CARE LOGS FOR CURRENT USER
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? myPlantId)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var logs = await _service.GetAllAsync(
                userId.Value,
                myPlantId);

            return Ok(logs.Select(ToResponse));
        }

        // GET: api/CareLogs/1
        // GET SPECIFIC CARE LOG
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var log = await _service.GetByIdAsync(
                id,
                userId.Value);

            if (log == null)
                return NotFound("Care log not found.");

            return Ok(ToResponse(log));
        }

        // POST: api/CareLogs
        // CREATE NEW CARE LOG
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CareLogRequest request)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (request.MyPlantId <= 0)
                return BadRequest("Invalid plant ID.");

            if (string.IsNullOrWhiteSpace(request.TaskName))
                return BadRequest("Task name is required.");

            var log = new CareLog
            {
                MyPlantId = request.MyPlantId,
                TaskName = request.TaskName.Trim(),
                CompletedAt = request.CompletedAt,
                Notes = request.Notes
            };

            var created = await _service.AddAsync(
                userId.Value,
                log);

            if (created == null)
                return NotFound("Plant not found.");

            // Retrieve the created log with plant information
            var saved = await _service.GetByIdAsync(
                created.CareLogId,
                userId.Value);

            if (saved == null)
            {
                return StatusCode(
                    500,
                    "Unable to retrieve the created care log.");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CareLogId },
                ToResponse(saved)
            );
        }

        // PUT: api/CareLogs/1
        // UPDATE EXISTING CARE LOG
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CareLogRequest request)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (request.MyPlantId <= 0)
                return BadRequest("Invalid plant ID.");

            if (string.IsNullOrWhiteSpace(request.TaskName))
                return BadRequest("Task name is required.");

            // Verify that the log belongs to current user
            var existing = await _service.GetByIdAsync(
                id,
                userId.Value);

            if (existing == null)
                return NotFound("Care log not found.");

            // Prevent changing the associated plant
            if (existing.MyPlantId != request.MyPlantId)
            {
                return BadRequest(
                    "MyPlantId cannot be changed.");
            }

            var updated = await _service.UpdateAsync(
                id,
                userId.Value,
                new CareLog
                {
                    TaskName = request.TaskName.Trim(),
                    CompletedAt = request.CompletedAt,
                    Notes = request.Notes
                });

            if (!updated)
                return NotFound("Care log not found.");

            return NoContent();
        }

        // DELETE: api/CareLogs/1
        // DELETE CARE LOG
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var deleted = await _service.DeleteAsync(
                id,
                userId.Value);

            if (!deleted)
                return NotFound("Care log not found.");

            return NoContent();
        }
    }
}