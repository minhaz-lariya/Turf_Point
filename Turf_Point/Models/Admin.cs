using System.ComponentModel.DataAnnotations;

namespace Turf_Point.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Adminname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
