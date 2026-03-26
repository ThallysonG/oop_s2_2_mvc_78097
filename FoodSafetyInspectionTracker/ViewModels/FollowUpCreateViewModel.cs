using System.ComponentModel.DataAnnotations;
using FoodSafetyInspectionTracker.Enums;

namespace FoodSafetyInspectionTracker.ViewModels
{
    public class FollowUpCreateViewModel
    {
        [Required]
        public int InspectionId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public FollowUpStatus Status { get; set; } = FollowUpStatus.Open;

        [DataType(DataType.Date)]
        public DateTime? ClosedDate { get; set; }
    }
}