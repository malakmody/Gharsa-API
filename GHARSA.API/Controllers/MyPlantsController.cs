
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
    public class MyPlantsController : ControllerBase
    {
        private readonly MyPlantService _service;

        public MyPlantsController(MyPlantService service)
        {
            _service = service;
        }

        // GET CURRENT USER ID FROM JWT
        private int? CurrentUserId()
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(claim, out var id))
                return id;

            return null;
        }

        // CONVERT MODEL TO RESPONSE DTO
        private static MyPlantResponse ToResponse(MyPlant mp)
        {
            return new MyPlantResponse
            {
                MyPlantId = mp.MyPlantId,
                PlantId = mp.PlantId,
                PlantName = mp.Plant?.Name ?? "",
                ImageUrl = mp.Plant?.ImageUrl,
                CategoryName = mp.Plant?.Category?.Name,
                Nickname = mp.Nickname,
                DateAdded = mp.DateAdded,
                LastWateredAt = mp.LastWateredAt
            };
        }

        // GET: api/MyPlants
        // GET ALL PLANTS FOR CURRENT USER
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var plants = await _service.GetAllAsync(
                userId.Value);

            return Ok(plants.Select(ToResponse));
        }

        // GET: api/MyPlants/1
        // GET SPECIFIC USER PLANT
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var plant = await _service.GetByIdAsync(
                id,
                userId.Value);

            if (plant == null)
                return NotFound("Plant not found.");

            return Ok(ToResponse(plant));
        }

        // POST: api/MyPlants
        // ADD PLANT TO USER COLLECTION
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] MyPlantRequest request)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            // CHECK IF PLANT EXISTS
            if (!await _service.PlantExistsAsync(
                request.PlantId))
            {
                return BadRequest(
                    "The selected plant does not exist.");
            }

            // PREVENT DUPLICATE PLANTS IN USER COLLECTION
            var userPlants = await _service.GetAllAsync(
                userId.Value);

            bool alreadyExists = userPlants.Any(
                p => p.PlantId == request.PlantId
            );

            if (alreadyExists)
            {
                return Conflict(
                    "This plant is already in your collection.");
            }

            // CREATE USER PLANT
            var myPlant = new MyPlant
            {
                UserId = userId.Value,
                PlantId = request.PlantId,
                Nickname = request.Nickname,
                LastWateredAt = null,
                DateAdded = DateTime.UtcNow
            };

            var created = await _service.AddAsync(myPlant);

            // RETRIEVE CREATED PLANT
            var saved = await _service.GetByIdAsync(
                created.MyPlantId,
                userId.Value);

            if (saved == null)
            {
                return StatusCode(
                    500,
                    "Unable to retrieve the created plant.");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.MyPlantId },
                ToResponse(saved)
            );
        }

        // PUT: api/MyPlants/1
        // UPDATE USER'S OWN PLANT
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] MyPlantRequest request)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return Unauthorized();

            var existing = await _service.GetByIdAsync(
                id,
                userId.Value);

            if (existing == null)
                return NotFound("Plant not found.");

            // PREVENT CHANGING PLANT ID
            if (existing.PlantId != request.PlantId)
            {
                return BadRequest(
                    "PlantId cannot be changed.");
            }

            var updated = await _service.UpdateAsync(
                id,
                userId.Value,
                new MyPlant
                {
                    Nickname = request.Nickname
                });

            if (!updated)
                return NotFound("Plant not found.");

            return NoContent();
        }

        // DELETE: api/MyPlants/1
        // DELETE USER'S OWN PLANT
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
                return NotFound("Plant not found.");

            return NoContent();
        }
    }
}
