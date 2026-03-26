using FoodSafetyInspectionTracker.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSafetyInspectionTracker.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public int InspectionId { get; set; }

        [ForeignKey(nameof(InspectionId))]
        public Inspection? Inspection { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public FollowUpStatus Status { get; set; } = FollowUpStatus.Open;

        [DataType(DataType.Date)]
        public DateTime? ClosedDate { get; set; }
    }
}