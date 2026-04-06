using System.ComponentModel.DataAnnotations;

namespace FoodSafetyInspectionTracker.Models
{
    public class Premise
    {
        [Required] public int Id { get; set; }
        [Required] public string? Name { get; set; } = "";
        [Required] public string? Address { get; set; } = "";
        [Required] public string? Town { get; set; } = "";
        [Required] public string? RiskRating { get; set; } = "";
        public List<Inspection>? Inspections { get; set; } = new();


    }
}
