using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class PlantCareTaskService
    {
        private readonly ApplicationDbContext _context;

        public PlantCareTaskService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL + FILTER BY PLANT
        public async Task<List<PlantCareTask>> GetAllAsync(
            int? plantId)
        {
            var query = _context.PlantCareTasks
                .Include(x => x.Plant)
                .Include(x => x.CareTask)
                .AsNoTracking()
                .AsQueryable();

            if (plantId.HasValue)
            {
                query = query.Where(x =>
                    x.PlantId == plantId.Value);
            }

            return await query
                .OrderBy(x => x.PlantId)
                .ThenBy(x => x.CareTaskId)
                .ToListAsync();
        }

        // GET BY COMPOSITE KEY
        public async Task<PlantCareTask?> GetByIdsAsync(
            int plantId,
            int careTaskId)
        {
            return await _context.PlantCareTasks
                .Include(x => x.Plant)
                .Include(x => x.CareTask)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.PlantId == plantId &&
                    x.CareTaskId == careTaskId);
        }

        // CHECK IF PLANT EXISTS
        public async Task<bool> PlantExistsAsync(int plantId)
        {
            return await _context.Plants
                .AnyAsync(p => p.PlantId == plantId);
        }

        // CHECK IF CARE TASK EXISTS
        public async Task<bool> CareTaskExistsAsync(
            int careTaskId)
        {
            return await _context.CareTasks
                .AnyAsync(t => t.CareTaskId == careTaskId);
        }

        // POST
        public async Task<PlantCareTask> AddAsync(
            PlantCareTask link)
        {
            _context.PlantCareTasks.Add(link);

            await _context.SaveChangesAsync();

            return link;
        }

        // PUT
        public async Task<bool> UpdateAsync(
            int plantId,
            int careTaskId,
            int? intervalDays)
        {
            var existing = await _context.PlantCareTasks
                .FindAsync(plantId, careTaskId);

            if (existing == null)
                return false;

            existing.IntervalDays = intervalDays;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(
            int plantId,
            int careTaskId)
        {
            var existing = await _context.PlantCareTasks
                .FindAsync(plantId, careTaskId);

            if (existing == null)
                return false;

            _context.PlantCareTasks.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}