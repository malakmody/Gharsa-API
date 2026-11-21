
using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CareTasksController : ControllerBase
    {
        private readonly CareTaskService _service;

        public CareTasksController(CareTaskService service)
        {
            _service = service;
        }

        // CONVERT MODEL TO RESPONSE DTO
        private static CareTaskResponse ToResponse(CareTask task)
        {
            return new CareTaskResponse
            {
                CareTaskId = task.CareTaskId,
                Name = task.Name,
                Description = task.Description
            };
        }

        // GET: api/CareTasks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _service.GetAllAsync();

            return Ok(tasks.Select(ToResponse));
        }

        // GET: api/CareTasks/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _service.GetByIdAsync(id);

            if (task == null)
                return NotFound("Care task not found.");

            return Ok(ToResponse(task));
        }

        // POST: api/CareTasks
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CareTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Task name is required.");

            var task = new CareTask
            {
                Name = request.Name.Trim(),
                Description = request.Description
            };

            var created = await _service.AddAsync(task);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CareTaskId },
                ToResponse(created)
            );
        }

        // PUT: api/CareTasks/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CareTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Task name is required.");

            var task = new CareTask
            {
                Name = request.Name.Trim(),
                Description = request.Description
            };

            var updated = await _service.UpdateAsync(id, task);

            if (!updated)
                return NotFound("Care task not found.");

            return NoContent();
        }

        // DELETE: api/CareTasks/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Care task not found.");

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete a task linked to plants."
                );
            }
        }
    }
}
