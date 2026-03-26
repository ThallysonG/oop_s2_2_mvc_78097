using FoodSafetyInspectionTracker.ViewModels;

namespace FoodSafetyInspectionTracker.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(string? town, string? riskRating);
    }
}