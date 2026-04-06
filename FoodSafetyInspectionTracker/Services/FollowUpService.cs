using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Services
{
    public class FollowUpService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FollowUpService> _logger;

        public FollowUpService(ApplicationDbContext context, ILogger<FollowUpService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<FollowUp>> GetAllAsync()
        {
            return await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premise)
                .OrderBy(f => f.Status)
                .ThenBy(f => f.DueDate)
                .ToListAsync();
        }

        public async Task<FollowUp?> GetByIdAsync(int id)
        {
            return await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premise)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task CreateAsync(FollowUp followUp)
        {
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            if (inspection == null)
            {
                _logger.LogError("FollowUp creation failed. Inspection not found. InspectionId: {InspectionId}",
                    followUp.InspectionId);
                throw new Exception("Inspection not found.");
            }

            if (followUp.DueDate < inspection.InspectionDate)
            {
                _logger.LogWarning("FollowUp due date before inspection date. InspectionId: {InspectionId}, DueDate: {DueDate}, InspectionDate: {InspectionDate}",
                    followUp.InspectionId, followUp.DueDate, inspection.InspectionDate);

                throw new Exception("The follow-up deadline cannot be earlier than the inspection date.");
            }

            if (followUp.Status == "Closed" && followUp.ClosedDate == null)
            {
                _logger.LogWarning("FollowUp closed without closed date. InspectionId: {InspectionId}",
                    followUp.InspectionId);
                throw new Exception("A closed follow-up must have a ClosedDate.");
            }

            _context.FollowUps.Add(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp created. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}, Status: {Status}",
                followUp.Id, followUp.InspectionId, followUp.Status);
        }

        public async Task UpdateAsync(FollowUp followUp)
        {
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            if (inspection == null)
            {
                _logger.LogError("FollowUp update failed. Inspection not found. InspectionId: {InspectionId}",
                    followUp.InspectionId);
                throw new Exception("Inspection not found.");
            }

            if (followUp.DueDate < inspection.InspectionDate)
            {
                _logger.LogWarning("FollowUp update invalid due date. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}",
                    followUp.Id, followUp.InspectionId);
                throw new Exception("The follow-up deadline cannot be earlier than the inspection date.");
            }

            if (followUp.Status == "Closed" && followUp.ClosedDate == null)
            {
                _logger.LogWarning("FollowUp update closed without closed date. FollowUpId: {FollowUpId}",
                    followUp.Id);
                throw new Exception("A closed follow-up must have a ClosedDate.");
            }

            _context.FollowUps.Update(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp updated. FollowUpId: {FollowUpId}, Status: {Status}",
                followUp.Id, followUp.Status);
        }

        public async Task CloseAsync(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                _logger.LogWarning("Attempt to close non-existing FollowUp. FollowUpId: {FollowUpId}", id);
                return;
            }

            followUp.Status = "Closed";
            followUp.ClosedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp closed. FollowUpId: {FollowUpId}, ClosedDate: {ClosedDate}",
                followUp.Id, followUp.ClosedDate);
        }

        public async Task DeleteAsync(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                _logger.LogWarning("Delete attempted for non-existing FollowUp. FollowUpId: {FollowUpId}", id);
                return;
            }

            _context.FollowUps.Remove(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp deleted. FollowUpId: {FollowUpId}", id);
        }

        public async Task<List<Inspection>> GetInspectionListAsync()
        {
            return await _context.Inspections
                .Include(i => i.Premise)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
        }
    }
}