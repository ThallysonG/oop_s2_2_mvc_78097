using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Enums;
using FoodSafetyInspectionTracker.Services.Interfaces;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Services
{
    public class DashboardService : IDashboardService
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
            var today = DateTime.Today;
            var startMonth = new DateTime(today.Year, today.Month, 1);
            var endMonth = startMonth.AddMonths(1);

            var premisesQuery = _context.Premises.AsQueryable();

            if (!string.IsNullOrWhiteSpace(town))
            {
                premisesQuery = premisesQuery.Where(p => p.Town == town);
            }

            if (!string.IsNullOrWhiteSpace(riskRating) &&
                Enum.TryParse<RiskRating>(riskRating, out var parsedRisk))
            {
                premisesQuery = premisesQuery.Where(p => p.RiskRating == parsedRisk);
            }

            var filteredPremisesIds = await premisesQuery.Select(p => p.Id).ToListAsync();

            var inspectionsThisMonth = await _context.Inspections
                .Where(i => i.InspectionDate >= startMonth && i.InspectionDate < endMonth)
                .Where(i => filteredPremisesIds.Contains(i.PremisesId))
                .CountAsync();

            var failedInspectionsThisMonth = await _context.Inspections
                .Where(i => i.InspectionDate >= startMonth && i.InspectionDate < endMonth)
                .Where(i => i.Outcome == InspectionOutcome.Fail)
                .Where(i => filteredPremisesIds.Contains(i.PremisesId))
                .CountAsync();

            var openOverdueFollowUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .Where(f => f.Status == FollowUpStatus.Open && f.DueDate < today)
                .Where(f => f.Inspection != null && filteredPremisesIds.Contains(f.Inspection.PremisesId))
                .CountAsync();

            _logger.LogInformation(
                "Dashboard loaded with filters Town={Town}, RiskRating={RiskRating}, InspectionsThisMonth={InspectionsThisMonth}, FailedInspectionsThisMonth={FailedInspectionsThisMonth}, OpenOverdueFollowUps={OpenOverdueFollowUps}",
                town, riskRating, inspectionsThisMonth, failedInspectionsThisMonth, openOverdueFollowUps);

            return new DashboardViewModel
            {
                Filters = new DashboardFilterViewModel
                {
                    Town = town,
                    RiskRating = Enum.TryParse<RiskRating>(riskRating, out var rr) ? rr : null
                },
                InspectionsThisMonth = inspectionsThisMonth,
                FailedInspectionsThisMonth = failedInspectionsThisMonth,
                OpenOverdueFollowUps = openOverdueFollowUps,
                Towns = await _context.Premises.Select(p => p.Town).Distinct().OrderBy(t => t).ToListAsync(),
                FilteredPremises = await premisesQuery.OrderBy(p => p.Name).ToListAsync()
            };
        }
    }
}