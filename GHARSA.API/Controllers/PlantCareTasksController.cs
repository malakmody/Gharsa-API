using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantCareTasksController : ControllerBase
    {
        private readonly PlantCareTaskService _service;

        public PlantCareTasksController(
            PlantCareTaskService service)
        {
            _service = service;
        }

        // CONVERT MODEL TO RESPONSE DTO
        private static PlantCareTaskResponse ToResponse(
            PlantCareTask link)
        {
            return new PlantCareTaskResponse
            {
                PlantId = link.PlantId,
                PlantName = link.Plant?.Name ?? "",
                CareTaskId = link.CareTaskId,
                CareTaskName = link.CareTask?.Name ?? "",
                IntervalDays = link.IntervalDays
            };
        }

        // GET: api/PlantCareTasks
        // GET: api/PlantCareTasks?plantId=1
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? plantId)
        {
            var links = await _service.GetAllAsync(plantId);

            return Ok(links.Select(ToResponse));
        }

        // GET: api/PlantCareTasks/1/2
        [HttpGet("{plantId:int}/{careTaskId:int}")]
        public async Task<IActionResult> GetByIds(
            int plantId,
            int careTaskId)
        {
            var link = await _service.GetByIdsAsync(
                plantId,
                careTaskId);

            if (link == null)
                return NotFound(
                    "Plant-care task link not found.");

            return Ok(ToResponse(link));
        }

        // POST: api/PlantCareTasks
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PlantCareTaskRequest request)
        {
            if (!await _service.PlantExistsAsync(
                request.PlantId))
            {
                return BadRequest(
                    "The selected plant does not exist.");
            }

            if (!await _service.CareTaskExistsAsync(
                request.CareTaskId))
            {
                return BadRequest(
                    "The selected care task does not exist.");
            }

            var existing = await _service.GetByIdsAsync(
                request.PlantId,
                request.CareTaskId);

            if (existing != null)
            {
                return Conflict(
                    "This care task is already linked to the plant.");
            }

            var link = new PlantCareTask
            {
                PlantId = request.PlantId,
                CareTaskId = request.CareTaskId,
                IntervalDays = request.IntervalDays
            };

            try
            {
                await _service.AddAsync(link);

                var saved = await _service.GetByIdsAsync(
                    link.PlantId,
                    link.CareTaskId);

                if (saved == null)
                    return StatusCode(
                        500,
                        "Unable to retrieve the created link.");

                return CreatedAtAction(
                    nameof(GetByIds),
                    new
                    {
                        plantId = link.PlantId,
                        careTaskId = link.CareTaskId
                    },
                    ToResponse(saved)
                );
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to create this plant-care task link.");
            }
        }

        // PUT: api/PlantCareTasks/1/2
        [HttpPut("{plantId:int}/{careTaskId:int}")]
        public async Task<IActionResult> Update(
            int plantId,
            int careTaskId,
            [FromBody] PlantCareTaskRequest request)
        {
            if (request.PlantId != plantId ||
                request.CareTaskId != careTaskId)
            {
                return BadRequest(
                    "Route IDs must match the request body IDs.");
            }

            var updated = await _service.UpdateAsync(
                plantId,
                careTaskId,
                request.IntervalDays);

            if (!updated)
                return NotFound(
                    "Plant-care task link not found.");

            return NoContent();
        }

        // DELETE: api/PlantCareTasks/1/2
        [HttpDelete("{plantId:int}/{careTaskId:int}")]
        public async Task<IActionResult> Delete(
            int plantId,
            int careTaskId)
        {
            var deleted = await _service.DeleteAsync(
                plantId,
                careTaskId);

            if (!deleted)
                return NotFound(
                    "Plant-care task link not found.");

            return NoContent();
        }
    }
}