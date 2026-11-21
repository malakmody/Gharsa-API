using GHARSA.API.DTOs;
using GHARSA.API.Models;
using GHARSA.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        // Convert Category Model to Response DTO
        private static CategoryResponse ToResponse(Category c)
        {
            return new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            };
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();

            return Ok(categories.Select(ToResponse));
        }

        // GET: api/Categories/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);

            if (category == null)
                return NotFound("Category not found.");

            return Ok(ToResponse(category));
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Category name is required.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description
            };

            var created = await _service.AddAsync(category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CategoryId },
                ToResponse(created)
            );
        }

        // PUT: api/Categories/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Category name is required.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description
            };

            var updated = await _service.UpdateAsync(
                id, category
            );

            if (!updated)
                return NotFound("Category not found.");

            return NoContent();
        }

        // DELETE: api/Categories/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Category not found.");

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete a category that has linked plants."
                );
            }
        }
    }
}