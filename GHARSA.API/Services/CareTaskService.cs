using GHARSA.API.Data;
using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class CareTaskService
    {
        private readonly ApplicationDbContext _context;

        public CareTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<CareTask>> GetAllAsync()
        {
            return await _context.CareTasks
                .AsNoTracking()
                .OrderBy(t => t.CareTaskId)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<CareTask?> GetByIdAsync(int id)
        {
            return await _context.CareTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.CareTaskId == id);
        }

        // POST
        public async Task<CareTask> AddAsync(CareTask task)
        {
            _context.CareTasks.Add(task);

            await _context.SaveChangesAsync();

            return task;
        }

        // PUT
        public async Task<bool> UpdateAsync(
            int id,
            CareTask task)
        {
            var existing = await _context.CareTasks
                .FindAsync(id);

            if (existing == null)
                return false;

            existing.Name = task.Name;
            existing.Description = task.Description;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.CareTasks
                .FindAsync(id);

            if (existing == null)
                return false;

            _context.CareTasks.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}