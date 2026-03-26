using FoodSafetyInspectionTracker.Models;

namespace FoodSafetyInspectionTracker.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardFilterViewModel Filters { get; set; } = new();
        public int InspectionsThisMonth { get; set; }
        public int FailedInspectionsThisMonth { get; set; }
        public int OpenOverdueFollowUps { get; set; }
        public List<string> Towns { get; set; } = new();
        public List<Premises> FilteredPremises { get; set; } = new();
    }
}