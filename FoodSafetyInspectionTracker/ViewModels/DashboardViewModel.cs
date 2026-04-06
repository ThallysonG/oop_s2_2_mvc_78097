namespace FoodSafetyInspectionTracker.ViewModels
{
    public class DashboardViewModel
    {
        public int InspectionsThisMonth { get; set; }
        public int FailedInspectionsThisMonth { get; set; }
        public int OverdueFollowUps { get; set; }

        public string? SelectedTown { get; set; }
        public string? SelectedRiskRating { get; set; }

        public List<string> AvailableTowns { get; set; } = new();
        public List<string> AvailableRiskRatings { get; set; } = new();
    }
}