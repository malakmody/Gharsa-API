
using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class MyPlantService
    {
        private readonly ApplicationDbContext _context;

        public MyPlantService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL PLANTS FOR A SPECIFIC USER
        public async Task<List<MyPlant>> GetAllAsync(int userId)
        {
            return await _context.MyPlants
                .Include(mp => mp.Plant)
                .ThenInclude(p => p.Category)
                .AsNoTracking()
                .Where(mp => mp.UserId == userId)
                .OrderByDescending(mp => mp.DateAdded)
                .ToListAsync();
        }

        // GET PLANT BY ID AND USER ID
        public async Task<MyPlant?> GetByIdAsync(
            int id,
            int userId)
        {
            return await _context.MyPlants
                .Include(mp => mp.Plant)
                .ThenInclude(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    mp => mp.MyPlantId == id &&
                          mp.UserId == userId);
        }

        // CHECK IF USER EXISTS
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        // CHECK IF PLANT EXISTS
        public async Task<bool> PlantExistsAsync(int plantId)
        {
            return await _context.Plants
                .AnyAsync(p => p.PlantId == plantId);
        }

        // CHECK IF PLANT ALREADY EXISTS IN USER COLLECTION
        public async Task<bool> ExistsForUserAsync(
            int userId,
            int plantId)
        {
            return await _context.MyPlants
                .AnyAsync(mp =>
                    mp.UserId == userId &&
                    mp.PlantId == plantId);
        }

        // ADD PLANT TO USER COLLECTION
        public async Task<MyPlant> AddAsync(MyPlant myPlant)
        {
            _context.MyPlants.Add(myPlant);

            await _context.SaveChangesAsync();

            return myPlant;
        }

        // UPDATE USER'S OWN PLANT
        // Only Nickname can be updated.
        // LastWateredAt is managed by CareLogService.
        public async Task<bool> UpdateAsync(
            int id,
            int userId,
            MyPlant myPlant)
        {
            var existing = await _context.MyPlants
                .FirstOrDefaultAsync(
                    mp => mp.MyPlantId == id &&
                          mp.UserId == userId);

            if (existing == null)
                return false;

            existing.Nickname = myPlant.Nickname;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE USER'S OWN PLANT
        public async Task<bool> DeleteAsync(
            int id,
            int userId)
        {
            var existing = await _context.MyPlants
                .FirstOrDefaultAsync(
                    mp => mp.MyPlantId == id &&
                          mp.UserId == userId);

            if (existing == null)
                return false;

            _context.MyPlants.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
