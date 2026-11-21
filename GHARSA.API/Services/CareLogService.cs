
using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class CareLogService
    {
        private readonly ApplicationDbContext _context;

        public CareLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL CARE LOGS FOR A SPECIFIC USER
        public async Task<List<CareLog>> GetAllAsync(
            int userId,
            int? myPlantId)
        {
            var query = _context.CareLogs
                .Include(log => log.MyPlant)
                .ThenInclude(mp => mp.Plant)
                .AsNoTracking()
                .Where(log => log.MyPlant.UserId == userId);

            if (myPlantId.HasValue)
            {
                query = query.Where(log =>
                    log.MyPlantId == myPlantId.Value);
            }

            return await query
                .OrderByDescending(log => log.CompletedAt)
                .ToListAsync();
        }

        // GET CARE LOG BY ID FOR A SPECIFIC USER
        public async Task<CareLog?> GetByIdAsync(
            int id,
            int userId)
        {
            return await _context.CareLogs
                .Include(log => log.MyPlant)
                .ThenInclude(mp => mp.Plant)
                .AsNoTracking()
                .FirstOrDefaultAsync(log =>
                    log.CareLogId == id &&
                    log.MyPlant.UserId == userId);
        }

        // ADD CARE LOG
        public async Task<CareLog?> AddAsync(
            int userId,
            CareLog careLog)
        {
            // Verify that the plant belongs to the user
            var myPlant = await _context.MyPlants
                .FirstOrDefaultAsync(mp =>
                    mp.MyPlantId == careLog.MyPlantId &&
                    mp.UserId == userId);

            if (myPlant == null)
                return null;

            _context.CareLogs.Add(careLog);

            await _context.SaveChangesAsync();

            // Update last watering date
            await UpdateLastWateredAtAsync(
                careLog.MyPlantId);

            return careLog;
        }

        // UPDATE CARE LOG
        public async Task<bool> UpdateAsync(
            int id,
            int userId,
            CareLog careLog)
        {
            var existing = await _context.CareLogs
                .Include(log => log.MyPlant)
                .FirstOrDefaultAsync(log =>
                    log.CareLogId == id &&
                    log.MyPlant.UserId == userId);

            if (existing == null)
                return false;

            existing.TaskName = careLog.TaskName;
            existing.CompletedAt = careLog.CompletedAt;
            existing.Notes = careLog.Notes;

            await _context.SaveChangesAsync();

            // Recalculate last watering date
            await UpdateLastWateredAtAsync(
                existing.MyPlantId);

            return true;
        }

        // DELETE CARE LOG
        public async Task<bool> DeleteAsync(
            int id,
            int userId)
        {
            var existing = await _context.CareLogs
                .Include(log => log.MyPlant)
                .FirstOrDefaultAsync(log =>
                    log.CareLogId == id &&
                    log.MyPlant.UserId == userId);

            if (existing == null)
                return false;

            int myPlantId = existing.MyPlantId;

            _context.CareLogs.Remove(existing);

            await _context.SaveChangesAsync();

            // Recalculate last watering date
            await UpdateLastWateredAtAsync(myPlantId);

            return true;
        }

        // RECALCULATE LAST WATERING DATE
        private async Task UpdateLastWateredAtAsync(
            int myPlantId)
        {
            var lastWateringDate = await _context.CareLogs
                .Where(log =>
                    log.MyPlantId == myPlantId &&
                    log.TaskName.ToLower() == "watering")
                .OrderByDescending(log => log.CompletedAt)
                .Select(log => (DateTime?)log.CompletedAt)
                .FirstOrDefaultAsync();

            var myPlant = await _context.MyPlants
                .FindAsync(myPlantId);

            if (myPlant == null)
                return;

            myPlant.LastWateredAt = lastWateringDate;

            await _context.SaveChangesAsync();
        }
    }
}
