using FoodSafetyInspectionTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(string? town, string? riskRating)
        {
            var model = await _dashboardService.GetDashboardAsync(town, riskRating);
            return View(model);
        }
    }
}