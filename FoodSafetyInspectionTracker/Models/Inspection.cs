using FoodSafetyInspectionTracker.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSafetyInspectionTracker.Models
{
    public class Inspection
    {
        public int Id { get; set; }

        [Required]
        public int PremisesId { get; set; }

        [ForeignKey(nameof(PremisesId))]
        public Premises? Premises { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InspectionDate { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public InspectionOutcome Outcome { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    }
}