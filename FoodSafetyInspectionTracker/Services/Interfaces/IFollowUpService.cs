using FoodSafetyInspectionTracker.ViewModels;

namespace FoodSafetyInspectionTracker.Services.Interfaces
{
    public interface IFollowUpService
    {
        Task CreateFollowUpAsync(FollowUpCreateViewModel vm, string? userName);
        Task CloseFollowUpAsync(int id, DateTime closedDate, string? userName);
        Task<List<int>> GetOverdueFollowUpIdsAsync();
    }
}