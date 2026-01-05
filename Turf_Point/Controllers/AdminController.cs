using Microsoft.AspNetCore.Mvc;

namespace Turf_Point.Controllers
{
    public class AdminController : Controller
    {

        [HttpGet("Admin/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("Admin/Dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Admin/Time-Slots")]
        public IActionResult TimeSlots()
        {
            return View();
        }

        [HttpGet("Admin/Customers")]
        public IActionResult Customers()
        {
            return View();
        }

        [HttpGet("Admin/upcomming-bookings")]
        public IActionResult upcommingBookings()
        {
            return View();
        }

        [HttpGet("Admin/todays-bookings")]
        public IActionResult todaysBookings()
        {
            return View();
        }

        [HttpGet("Admin/complete-bookings")]
        public IActionResult completeBookings()
        {
            return View();
        }

        [HttpGet("Admin/bookings/details/{id}")]
        public IActionResult bookingDetails(int id)
        {
            ViewBag.bookingId = id;
            return View();
        }

        [HttpGet("Admin/Payments")]
        public IActionResult Payments()
        {
            return View();
        }

        [HttpGet("Admin/Todays/Available-Slots")]
        public IActionResult todaysAvailableSlotes()
        {
            return View();
        }

        [HttpGet("Admin/Available-Slots")]
        public IActionResult availableSlotes()
        {
            return View();
        }

        [HttpGet("Admin/Change-Password")]
        public IActionResult changePassword()
        {
            return View();
        }
    }
}
