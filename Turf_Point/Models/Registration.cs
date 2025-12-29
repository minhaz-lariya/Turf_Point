using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turf_Point.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username (Email) is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty; // ⚠️ Store hash in production
        public DateTime Created_At { get; set; } = DateTime.Now;
        public string token { get; set; } = string.Empty;

    }

    [NotMapped]
    public class Authentication
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
    }
}
