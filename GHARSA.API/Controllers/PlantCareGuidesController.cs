
using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantCareGuidesController : ControllerBase
    {
        private readonly PlantCareGuideService _service;

        public PlantCareGuidesController(
            PlantCareGuideService service)
        {
            _service = service;
        }

        // CONVERT MODEL TO RESPONSE DTO
        private static PlantCareGuideResponse ToResponse(
            PlantCareGuide guide)
        {
            return new PlantCareGuideResponse
            {
                PlantCareGuideId = guide.PlantCareGuideId,
                PlantId = guide.PlantId,
                PlantName = guide.Plant?.Name ?? "",
                SunlightRequirement = guide.SunlightRequirement,
                WaterRequirement = guide.WaterRequirement,
                SoilType = guide.SoilType,
                WateringIntervalDays = guide.WateringIntervalDays,
                AdditionalTips = guide.AdditionalTips
            };
        }

        // GET: api/PlantCareGuides
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guides = await _service.GetAllAsync();

            return Ok(guides.Select(ToResponse));
        }

        // GET: api/PlantCareGuides/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id)
        {
            var guide = await _service.GetByIdAsync(id);

            if (guide == null)
                return NotFound("Care guide not found.");

            return Ok(ToResponse(guide));
        }

        // GET: api/PlantCareGuides/plant/1
        [HttpGet("plant/{plantId:int}")]
        public async Task<IActionResult> GetByPlantId(
            [FromRoute] int plantId)
        {
            var guide = await _service.GetByPlantIdAsync(
                plantId);

            if (guide == null)
                return NotFound(
                    "No care guide found for this plant.");

            return Ok(ToResponse(guide));
        }

        // POST: api/PlantCareGuides
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PlantCareGuideRequest request)
        {
            if (!await _service.PlantExistsAsync(
                request.PlantId))
            {
                return BadRequest(
                    "The selected plant does not exist.");
            }

            // Enforce One-to-One relationship
            var existingGuide =
                await _service.GetByPlantIdAsync(
                    request.PlantId);

            if (existingGuide != null)
            {
                return Conflict(
                    "This plant already has a care guide.");
            }

            var guide = new PlantCareGuide
            {
                PlantId = request.PlantId,
                SunlightRequirement =
                    request.SunlightRequirement,
                WaterRequirement =
                    request.WaterRequirement,
                SoilType = request.SoilType,
                WateringIntervalDays =
                    request.WateringIntervalDays,
                AdditionalTips = request.AdditionalTips
            };

            try
            {
                var created = await _service.AddAsync(guide);

                var saved = await _service.GetByIdAsync(
                    created.PlantCareGuideId);

                if (saved == null)
                    return StatusCode(
                        500,
                        "Unable to retrieve the created care guide.");

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.PlantCareGuideId },
                    ToResponse(saved)
                );
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to create the care guide. " +
                    "Check whether the plant already has one.");
            }
        }

        // PUT: api/PlantCareGuides/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] PlantCareGuideRequest request)
        {
            var currentGuide =
                await _service.GetByIdAsync(id);

            if (currentGuide == null)
                return NotFound("Care guide not found.");

            // Do not move an existing guide to another plant
            if (currentGuide.PlantId != request.PlantId)
            {
                return BadRequest(
                    "PlantId cannot be changed. " +
                    "Delete this guide and create a new one.");
            }

            var guide = new PlantCareGuide
            {
                PlantId = currentGuide.PlantId,
                SunlightRequirement =
                    request.SunlightRequirement,
                WaterRequirement =
                    request.WaterRequirement,
                SoilType = request.SoilType,
                WateringIntervalDays =
                    request.WateringIntervalDays,
                AdditionalTips = request.AdditionalTips
            };

            var updated = await _service.UpdateAsync(
                id, guide);

            if (!updated)
                return NotFound("Care guide not found.");

            return NoContent();
        }

        // DELETE: api/PlantCareGuides/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Care guide not found.");

            return NoContent();
        }
    }
}
