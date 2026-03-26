using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services.Interfaces;
using FoodSafetyInspectionTracker.ViewModels;

namespace FoodSafetyInspectionTracker.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionService> _logger;

        public InspectionService(ApplicationDbContext context, ILogger<InspectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateInspectionAsync(InspectionCreateViewModel vm, string? userName)
        {
            var inspection = new Inspection
            {
                PremisesId = vm.PremisesId,
                InspectionDate = vm.InspectionDate,
                Score = vm.Score,
                Outcome = vm.Outcome,
                Notes = vm.Notes
            };

            _context.Inspections.Add(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Inspection created by {UserName}. PremisesId={PremisesId}, InspectionId={InspectionId}, Score={Score}, Outcome={Outcome}",
                userName ?? "Anonymous", inspection.PremisesId, inspection.Id, inspection.Score, inspection.Outcome);
        }
    }
}