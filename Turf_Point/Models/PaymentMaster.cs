using System.ComponentModel.DataAnnotations;

namespace Turf_Point.Models
{
    public class PaymentMaster
    {
        public int Id { get; set; }
        public virtual BookingMaster? BookingMaster { get; set; }
        [Required(ErrorMessage = "BookingId is required")]
        public int BookingMasterId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentType { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime Created_At { get; set; } = DateTime.Now;
    }
}
