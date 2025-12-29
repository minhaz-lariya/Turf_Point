using System.ComponentModel.DataAnnotations;

namespace Turf_Point.Models
{
    public class BookingSlot
    {
        public int Id { get; set; }
        public BookingMaster? BookingMaster { get; set; }
        public int BookingMasterId { get; set; } 
        public Timeslot? Timeslot { get; set; }

        [Required(ErrorMessage = "TimeslotId is required")]
        public int TimeslotId { get; set; } // FK to Timeslot

        [Range(0, double.MaxValue, ErrorMessage = "Rate must be a positive number")]
        public double Rate { get; set; }
        public string? Status { get; set; } = "Confirm";
        public DateTime Created_At { get; set; } = DateTime.Now;
    }
}
