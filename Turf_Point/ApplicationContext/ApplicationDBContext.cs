using Microsoft.EntityFrameworkCore;
using Turf_Point.Models;

namespace Turf_Point.ApplicationContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<BookingMaster> BookingMasters { get; set; }
        public DbSet<BookingSlot> BookingSlots { get; set; }
        public DbSet<Registration> registrations { get; set; }
        public DbSet<Timeslot> timeslots { get; set; }
        public DbSet<PaymentMaster> paymentMasters { get; set; }
        public DbSet<Admin> Admins { get; set; }

    }
}
