using System.ComponentModel.DataAnnotations;

namespace Turf_Point.Models
{
    public class BookingMaster
    {
        public BookingMaster()
        {
            Slots = new HashSet<BookingSlot>();
            Payments = new HashSet<PaymentMaster>();
        }

        public int Id { get; set; }
        
        public virtual Registration? Registration { get; set; }

        [Required(ErrorMessage = "RegistrationId is required")]
        public int RegistrationId { get; set; }     // FK to Registration
        public ICollection<BookingSlot> Slots { get; set; }
        public ICollection<PaymentMaster> Payments { get; set; }
        [Required(ErrorMessage = "Booking date is required")]
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = System.DateTime.Now;
    }
}
