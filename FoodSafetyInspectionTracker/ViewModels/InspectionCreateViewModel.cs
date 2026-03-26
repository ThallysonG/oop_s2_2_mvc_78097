using System.ComponentModel.DataAnnotations;
using FoodSafetyInspectionTracker.Enums;

namespace FoodSafetyInspectionTracker.ViewModels
{
    public class InspectionCreateViewModel
    {
        [Required]
        public int PremisesId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InspectionDate { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public InspectionOutcome Outcome { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}