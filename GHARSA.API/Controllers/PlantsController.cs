using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly PlantService _service;

        public PlantsController(PlantService service)
        {
            _service = service;
        }

        // Convert Plant Model to Response DTO
        private static PlantResponse ToResponse(Plant p)
        {
            return new PlantResponse
            {
                PlantId = p.PlantId,
                Name = p.Name,
                ScientificName = p.ScientificName,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "",
                SunlightRequirement =
                    p.PlantCareGuide?.SunlightRequirement,
                WaterRequirement =
                    p.PlantCareGuide?.WaterRequirement,
                WateringIntervalDays =
                    p.PlantCareGuide?.WateringIntervalDays
            };
        }

        // GET: api/Plants
        // GET: api/Plants?search=Rose
        // GET: api/Plants?categoryId=1
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? categoryId)
        {
            var plants = await _service.GetAllAsync(
                search, categoryId
            );

            return Ok(plants.Select(ToResponse));
        }

        // GET: api/Plants/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id)
        {
            var plant = await _service.GetByIdAsync(id);

            if (plant == null)
                return NotFound("Plant not found.");

            return Ok(ToResponse(plant));
        }

        // POST: api/Plants
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PlantRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Plant name is required.");

            if (!await _service.CategoryExistsAsync(
                request.CategoryId))
            {
                return BadRequest(
                    "The selected category does not exist."
                );
            }

            var plant = new Plant
            {
                Name = request.Name.Trim(),
                ScientificName = request.ScientificName,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId
            };

            var created = await _service.AddAsync(plant);

            var saved = await _service.GetByIdAsync(
                created.PlantId
            );

            if (saved == null)
                return StatusCode(500, "Unable to retrieve the created plant.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.PlantId },
                ToResponse(saved)
            );
        }

        // PUT: api/Plants/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] PlantRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Plant name is required.");

            if (!await _service.CategoryExistsAsync(
                request.CategoryId))
            {
                return BadRequest(
                    "The selected category does not exist."
                );
            }

            var plant = new Plant
            {
                Name = request.Name.Trim(),
                ScientificName = request.ScientificName,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId
            };

            var updated = await _service.UpdateAsync(
                id, plant
            );

            if (!updated)
                return NotFound("Plant not found.");

            return NoContent();
        }

        // DELETE: api/Plants/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Plant not found.");

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete this plant because it has linked records."
                );
            }
        }
    }
}