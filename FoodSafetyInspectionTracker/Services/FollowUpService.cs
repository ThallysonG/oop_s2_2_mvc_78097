using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Enums;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services.Interfaces;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Services
{
    public class FollowUpService : IFollowUpService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FollowUpService> _logger;

        public FollowUpService(ApplicationDbContext context, ILogger<FollowUpService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateFollowUpAsync(FollowUpCreateViewModel vm, string? userName)
        {
            var inspection = await _context.Inspections.FirstOrDefaultAsync(i => i.Id == vm.InspectionId);

            if (inspection == null)
            {
                _logger.LogWarning("Attempt to create FollowUp for non-existing InspectionId={InspectionId}", vm.InspectionId);
                throw new InvalidOperationException("Inspection not found.");
            }

            if (vm.DueDate < inspection.InspectionDate)
            {
                _logger.LogWarning(
                    "Business rule violation by {UserName}. FollowUp DueDate={DueDate} before InspectionDate={InspectionDate}. InspectionId={InspectionId}",
                    userName ?? "Anonymous", vm.DueDate, inspection.InspectionDate, vm.InspectionId);

                throw new InvalidOperationException("Due date cannot be before inspection date.");
            }

            if (vm.Status == FollowUpStatus.Closed && vm.ClosedDate == null)
            {
                _logger.LogWarning(
                    "Business rule violation by {UserName}. FollowUp closed without ClosedDate. InspectionId={InspectionId}",
                    userName ?? "Anonymous", vm.InspectionId);

                throw new InvalidOperationException("Closed follow-up requires a closed date.");
            }

            var followUp = new FollowUp
            {
                InspectionId = vm.InspectionId,
                DueDate = vm.DueDate,
                Status = vm.Status,
                ClosedDate = vm.ClosedDate
            };

            _context.FollowUps.Add(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "FollowUp created by {UserName}. InspectionId={InspectionId}, FollowUpId={FollowUpId}, DueDate={DueDate}, Status={Status}",
                userName ?? "Anonymous", followUp.InspectionId, followUp.Id, followUp.DueDate, followUp.Status);
        }

        public async Task CloseFollowUpAsync(int id, DateTime closedDate, string? userName)
        {
            var followUp = await _context.FollowUps.FirstOrDefaultAsync(f => f.Id == id);

            if (followUp == null)
            {
                _logger.LogWarning("Attempt to close non-existing FollowUpId={FollowUpId}", id);
                throw new InvalidOperationException("Follow-up not found.");
            }

            followUp.Status = FollowUpStatus.Closed;
            followUp.ClosedDate = closedDate;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "FollowUp closed by {UserName}. FollowUpId={FollowUpId}, ClosedDate={ClosedDate}",
                userName ?? "Anonymous", id, closedDate);
        }

        public async Task<List<int>> GetOverdueFollowUpIdsAsync()
        {
            var today = DateTime.Today;

            return await _context.FollowUps
                .Where(f => f.Status == FollowUpStatus.Open && f.DueDate < today)
                .Select(f => f.Id)
                .ToListAsync();
        }
    }
}