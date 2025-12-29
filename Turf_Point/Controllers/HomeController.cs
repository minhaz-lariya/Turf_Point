using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Turf_Point.Models;

namespace Turf_Point.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

        [HttpGet("Sign-Up")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet("Sign-In")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet("Time-Slots")]
        public IActionResult TimeSlots()
        {
            return View();
        }

        [HttpGet("Bookings")]
        public IActionResult Bookings()
        {
            return View();
        }

    }
}
