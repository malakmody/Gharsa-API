using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class PlantService
    {
        private readonly ApplicationDbContext _context;

        public PlantService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL + SEARCH + FILTER
        public async Task<List<Plant>> GetAllAsync(
            string? search,
            int? categoryId)
        {
            var query = _context.Plants
                .Include(p => p.Category)
                .Include(p => p.PlantCareGuide)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(keyword) ||
                    (p.ScientificName != null &&
                     p.ScientificName.Contains(keyword))
                );
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value
                );
            }

            return await query
                .OrderBy(p => p.PlantId)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<Plant?> GetByIdAsync(int id)
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.PlantCareGuide)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlantId == id);
        }

        // CHECK CATEGORY
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryId == categoryId);
        }

        // POST
        public async Task<Plant> AddAsync(Plant plant)
        {
            _context.Plants.Add(plant);

            await _context.SaveChangesAsync();

            return plant;
        }

        // PUT
        public async Task<bool> UpdateAsync(
            int id,
            Plant plant)
        {
            var existing = await _context.Plants
                .FindAsync(id);

            if (existing == null)
                return false;

            existing.Name = plant.Name;
            existing.ScientificName = plant.ScientificName;
            existing.Description = plant.Description;
            existing.ImageUrl = plant.ImageUrl;
            existing.CategoryId = plant.CategoryId;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Plants
                .FindAsync(id);

            if (existing == null)
                return false;

            _context.Plants.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}