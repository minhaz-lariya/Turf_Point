using Microsoft.AspNetCore.Authorization;  // Add if you want to secure endpoints
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turf_Point.ApplicationContext;
using Turf_Point.Models;

namespace Turf_Point.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDBContext _dbcontext;
        
        public AdminController(ApplicationDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Admin admin)
        {
            try
            {
                var data = await _dbcontext.Admins
                    .Where(o => o.Username == admin.Username && o.Password == admin.Password)
                    .Select(o => new
                    {
                        o.Username,
                        o.AdminId,
                        o.Email
                    }).FirstOrDefaultAsync();

                if (data != null)
                {
                    return Ok(new { Status = "Ok", Result = data });
                }
                else
                {
                    return Ok(new { Status = "Fail", Results = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Fail", Results = ex.Message });
            }
        }


        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var timeSlotes = await _dbcontext.timeslots.CountAsync();
                var users = await _dbcontext.registrations.CountAsync();
                var bookings = await _dbcontext.BookingMasters.CountAsync();

                var todaysbooking = await _dbcontext.BookingMasters
                                                    .Where(o=> o.BookingDate.Date == System.DateTime.Today.Date)
                                                    .CountAsync();

                var todaysbookingSlots = await (from A in _dbcontext.BookingSlots
                                                join B in _dbcontext.BookingMasters on A.BookingMasterId equals B.Id
                                                where (B.BookingDate.Date == System.DateTime.Today.Date)
                                                select new
                                                {
                                                    A.Id
                                                }).CountAsync();

                var revenue = await (from A in _dbcontext.BookingSlots
                                        join B in _dbcontext.BookingMasters on A.BookingMasterId equals B.Id
                                        select new
                                        {
                                            A.Rate
                                        }).SumAsync(o=> o.Rate);

                var last7DaysSales = await (from A in _dbcontext.BookingSlots
                                            join B in _dbcontext.BookingMasters on A.BookingMasterId equals B.Id
                                            where B.BookingDate.Date >= DateTime.Today.AddDays(-6)
                                            group A by B.BookingDate.Date into g
                                            orderby g.Key
                                            select new
                                            {
                                                Date = g.Key.ToString("yyyy-MM-dd"),
                                                DayName = g.Key.ToString("dddd"), // Monday, Tuesday, etc.
                                                TotalSales = g.Sum(x => x.Rate)
                                            }).ToListAsync();


                var todaysRemainSlots = (timeSlotes - todaysbookingSlots);
                var payments = _dbcontext.paymentMasters.Sum(o => (o != null ? o.Amount : 0));
                var dues = (Convert.ToDecimal(revenue) - payments);

                return Ok(new { Status = "Ok", Results = new { dues, payments, timeSlotes, users, bookings, todaysbooking, todaysbookingSlots, todaysRemainSlots, last7DaysSales, revenue } });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Fail", Results = ex.Message });
            }
        }


        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
                    return BadRequest(new { Status = "Fail", Message = "All fields are required." });

                var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.AdminId == model.Id && a.Password == model.OldPassword);
                if (admin == null)
                    return Ok(new { Status = "Fail", Message = "Invalid username or old password." });

                admin.Password = model.NewPassword;
                await _dbcontext.SaveChangesAsync();

                return Ok(new { Status = "Ok", Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Fail", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ChangeProfile")]
        public async Task<IActionResult> ChangeProfile([FromBody] ChangeProfileModel model)
        {
            try
            {
                var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.AdminId == model.Id);
                if (admin == null)
                    return Ok(new { Status = "Fail", Message = "Admin not found." });

                if (!string.IsNullOrEmpty(model.NewUsername))
                    admin.Username = model.NewUsername;

                if (!string.IsNullOrEmpty(model.Email))
                    admin.Email = model.Email;

                await _dbcontext.SaveChangesAsync();

                return Ok(new { Status = "Ok", Message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Fail", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("ViewProfile/{Id}")]
        public async Task<IActionResult> ViewProfile(int Id)
        {
            try
            { 
                var admin = await _dbcontext.Admins
                    .Where(a => a.AdminId == Id)
                    .Select(a => new
                    {
                        a.AdminId,
                        a.Username,
                        a.Email
                    })
                    .FirstOrDefaultAsync();

                if (admin == null)
                    return Ok(new { Status = "Fail", Message = "Admin not found." });

                return Ok(new { Status = "Ok", Result = admin });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Fail", Message = ex.Message });
            }
        }
    }

    public class ChangePasswordModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ChangeProfileModel
    {
        public int Id { get; set; }     
        public string? NewUsername { get; set; }            
        public string? Email { get; set; }                  
    }
}
