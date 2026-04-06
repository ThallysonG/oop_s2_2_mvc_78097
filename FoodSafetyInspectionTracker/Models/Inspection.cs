using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FoodSafetyInspectionTracker.Models
{
    public class Inspection
    {
        public int Id { get; set; }
        [Required] public int PremiseId { get; set; }
        [ForeignKey("PremiseId")] public Premise? Premise { get; set; }
        [Required] public DateTime InspectionDate { get; set; }
        [Range(0, 100)] public int Score { get; set; }
        [Required] public string? Outcome { get; set; } = "";
        public string? Notes { get; set; }
        public List<FollowUp>? FollowUps { get; set; } = new();


    }
}
