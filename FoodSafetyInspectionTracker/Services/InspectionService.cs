using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafetyInspectionTracker.Services
{
    public class InspectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionService> _logger;

        public InspectionService(ApplicationDbContext context, ILogger<InspectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Inspection>> GetAllAsync()
        {
            return await _context.Inspections
                .Include(i => i.Premise)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
        }

        public async Task<Inspection?> GetByIdAsync(int id)
        {
            return await _context.Inspections
                .Include(i => i.Premise)
                .Include(i => i.FollowUps)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CreateAsync(Inspection inspection)
        {
            if (inspection.Score < 0 || inspection.Score > 100)
            {
                _logger.LogWarning("Invalid inspection score. PremisesId: {PremisesId}, Score: {Score}",
                    inspection.PremiseId, inspection.Score);
                throw new Exception("O score da inspeção deve estar entre 0 e 100.");
            }

            if (inspection.Outcome != "Pass" && inspection.Outcome != "Fail")
            {
                _logger.LogWarning("Invalid inspection outcome. PremisesId: {PremisesId}, Outcome: {Outcome}",
                    inspection.PremiseId, inspection.Outcome);
                throw new Exception("O resultado da inspeção deve ser Pass ou Fail.");
            }

            _context.Inspections.Add(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection created. InspectionId: {InspectionId}, PremisesId: {PremisesId}, Outcome: {Outcome}",
                inspection.Id, inspection.PremiseId, inspection.Outcome);
        }

        public async Task UpdateAsync(Inspection inspection)
        {
            if (inspection.Score < 0 || inspection.Score > 100)
            {
                _logger.LogWarning("Invalid inspection score on update. InspectionId: {InspectionId}, Score: {Score}",
                    inspection.Id, inspection.Score);
                throw new Exception("O score da inspeção deve estar entre 0 e 100.");
            }

            if (inspection.Outcome != "Pass" && inspection.Outcome != "Fail")
            {
                _logger.LogWarning("Invalid inspection outcome on update. InspectionId: {InspectionId}, Outcome: {Outcome}",
                    inspection.Id, inspection.Outcome);
                throw new Exception("O resultado da inspeção deve ser Pass ou Fail.");
            }

            _context.Inspections.Update(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection updated. InspectionId: {InspectionId}, PremisesId: {PremisesId}",
                inspection.Id, inspection.PremiseId);
        }

        public async Task DeleteAsync(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                _logger.LogWarning("Delete attempted for non-existing Inspection. InspectionId: {InspectionId}", id);
                return;
            }

            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection deleted. InspectionId: {InspectionId}", id);
        }

        public async Task<List<Premise>> GetPremisesListAsync()
        {
            return await _context.Premises
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}