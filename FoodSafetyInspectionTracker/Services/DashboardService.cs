using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(string? town, string? riskRating)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var today = now.Date;

            var inspectionsQuery = _context.Inspections
                .Include(i => i.Premise)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(town))
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.Premise != null && i.Premise.Town == town);
            }

            if (!string.IsNullOrWhiteSpace(riskRating))
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.Premise != null && i.Premise.RiskRating == riskRating);
            }

            var followUpsQuery = _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premise)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(town))
            {
                followUpsQuery = followUpsQuery.Where(f =>
                    f.Inspection != null &&
                    f.Inspection.Premise != null &&
                    f.Inspection.Premise.Town == town);
            }

            if (!string.IsNullOrWhiteSpace(riskRating))
            {
                followUpsQuery = followUpsQuery.Where(f =>
                    f.Inspection != null &&
                    f.Inspection.Premise != null &&
                    f.Inspection.Premise.RiskRating == riskRating);
            }

            var model = new DashboardViewModel
            {
                SelectedTown = town,
                SelectedRiskRating = riskRating,

                InspectionsThisMonth = await inspectionsQuery
                    .CountAsync(i => i.InspectionDate >= firstDayOfMonth),

                FailedInspectionsThisMonth = await inspectionsQuery
                    .CountAsync(i => i.InspectionDate >= firstDayOfMonth && i.Outcome == "Fail"),

                OverdueFollowUps = await followUpsQuery
                    .CountAsync(f => f.DueDate < today && f.Status == "Open"),

                AvailableTowns = await _context.Premises
                    .Where(p => p.Town != null)
                    .Select(p => p.Town!)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToListAsync(),

                AvailableRiskRatings = await _context.Premises
                    .Where(p => p.RiskRating != null)
                    .Select(p => p.RiskRating!)
                    .Distinct()
                    .OrderBy(r => r)
                    .ToListAsync()
            };

            _logger.LogInformation(
                "Dashboard loaded. Town: {Town}, RiskRating: {RiskRating}, InspectionsThisMonth: {InspectionsThisMonth}, FailedInspectionsThisMonth: {FailedInspectionsThisMonth}, OverdueFollowUps: {OverdueFollowUps}",
                town, riskRating, model.InspectionsThisMonth, model.FailedInspectionsThisMonth, model.OverdueFollowUps);

            return model;
        }
    }
}