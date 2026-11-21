using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class PlantCareGuideService
    {
        private readonly ApplicationDbContext _context;

        public PlantCareGuideService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<PlantCareGuide>> GetAllAsync()
        {
            return await _context.PlantCareGuides
                .Include(g => g.Plant)
                .AsNoTracking()
                .OrderBy(g => g.PlantCareGuideId)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<PlantCareGuide?> GetByIdAsync(int id)
        {
            return await _context.PlantCareGuides
                .Include(g => g.Plant)
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.PlantCareGuideId == id);
        }

        // GET GUIDE BY PLANT ID
        public async Task<PlantCareGuide?> GetByPlantIdAsync(
            int plantId)
        {
            return await _context.PlantCareGuides
                .Include(g => g.Plant)
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.PlantId == plantId);
        }

        // CHECK IF PLANT EXISTS
        public async Task<bool> PlantExistsAsync(int plantId)
        {
            return await _context.Plants
                .AnyAsync(p => p.PlantId == plantId);
        }

        // POST
        public async Task<PlantCareGuide> AddAsync(
            PlantCareGuide guide)
        {
            _context.PlantCareGuides.Add(guide);

            await _context.SaveChangesAsync();

            return guide;
        }

        // PUT
        public async Task<bool> UpdateAsync(
            int id,
            PlantCareGuide guide)
        {
            var existing = await _context.PlantCareGuides
                .FindAsync(id);

            if (existing == null)
                return false;

            existing.SunlightRequirement =
                guide.SunlightRequirement;

            existing.WaterRequirement =
                guide.WaterRequirement;

            existing.SoilType = guide.SoilType;

            existing.WateringIntervalDays =
                guide.WateringIntervalDays;

            existing.AdditionalTips =
                guide.AdditionalTips;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.PlantCareGuides
                .FindAsync(id);

            if (existing == null)
                return false;

            _context.PlantCareGuides.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}