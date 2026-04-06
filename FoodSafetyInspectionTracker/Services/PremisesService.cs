using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafetyInspectionTracker.Services
{
    public class PremisesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PremisesService> _logger;

        public PremisesService(ApplicationDbContext context, ILogger<PremisesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Premise>> GetAllAsync()
        {
            return await _context.Premises
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Premise?> GetByIdAsync(int id)
        {
            return await _context.Premises
                .Include(p => p.Inspections)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(Premise premises)
        {
            _context.Premises.Add(premises);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Premises created. PremisesId: {PremisesId}, Town: {Town}, RiskRating: {RiskRating}",
                premises.Id, premises.Town, premises.RiskRating);
        }

        public async Task UpdateAsync(Premise premises)
        {
            _context.Premises.Update(premises);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Premises updated. PremisesId: {PremisesId}", premises.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var premises = await _context.Premises.FindAsync(id);
            if (premises == null)
            {
                _logger.LogWarning("Delete attempted for non-existing Premises. PremisesId: {PremisesId}", id);
                return;
            }

            _context.Premises.Remove(premises);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Premises deleted. PremisesId: {PremisesId}", id);
        }
    }
}