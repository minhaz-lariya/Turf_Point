using System.ComponentModel.DataAnnotations;

namespace Turf_Point.Models
{
    public class Timeslot
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Timename is required")]
        [StringLength(50, ErrorMessage = "Timename cannot be longer than 50 characters")]
        public string Timename { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Rate must be a positive value")]
        public double Rate { get; set; }

        public TimeOnly LastBookingTime { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Morning|Afternoon|Evening|Night)$", ErrorMessage = "Status must be Morning, Afternoon, Evening or Night")]
        public string Type { get; set; } = string.Empty;
        [Required(ErrorMessage = "Type is required")]
        public string Status { get; set; } = string.Empty;
    }
}
