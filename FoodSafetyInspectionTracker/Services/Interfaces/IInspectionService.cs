using FoodSafetyInspectionTracker.ViewModels;

namespace FoodSafetyInspectionTracker.Services.Interfaces
{
    public interface IInspectionService
    {
        Task CreateInspectionAsync(InspectionCreateViewModel vm, string? userName);
    }
}