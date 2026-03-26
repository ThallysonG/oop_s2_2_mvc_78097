using FoodSafetyInspectionTracker.Enums;

namespace FoodSafetyInspectionTracker.ViewModels
{
    public class DashboardFilterViewModel
    {
        public string? Town { get; set; }
        public RiskRating? RiskRating { get; set; }
    }
}