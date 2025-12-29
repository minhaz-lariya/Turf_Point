using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turf_Point.ApplicationContext;
using Turf_Point.Models;

namespace Turf_Point.Controllers.apis
{
    [Route("api/BookingMaster")]
    [ApiController]
    public class BookingMasterController : ControllerBase
    {
        private readonly ApplicationDBContext _dbcontext;

        public BookingMasterController(ApplicationDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // ✅ GET all bookings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var bookings = await _dbcontext.BookingMasters.ToListAsync();
                return Ok(new { Status = "OK", Data = bookings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ GET booking by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var booking = await _dbcontext.BookingMasters.FindAsync(id);
                if (booking == null)
                    return NotFound(new { Status = "Error", Message = "Booking not found" });

                return Ok(new { Status = "OK", Data = booking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ CREATE new booking
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingMaster booking)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Status = "Error", Message = "Invalid data", Errors = ModelState });

                // Ensure AllocationDate is not before BookingDate
                if (booking.BookingDate < booking.BookingDate)
                    return BadRequest(new { Status = "Error", Message = "Allocation date cannot be earlier than booking date" });

                _dbcontext.BookingMasters.Add(booking);
                await _dbcontext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "Booking created successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ UPDATE booking
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingMaster updatedBooking)
        {
            try
            {
                if (id != updatedBooking.Id)
                    return BadRequest(new { Status = "Error", Message = "Id mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(new { Status = "Error", Message = "Invalid data", Errors = ModelState });

                var booking = await _dbcontext.BookingMasters.FindAsync(id);
                if (booking == null)
                    return NotFound(new { Status = "Error", Message = "Booking not found" });

                if (updatedBooking.BookingDate < updatedBooking.BookingDate)
                    return BadRequest(new { Status = "Error", Message = "Allocation date cannot be earlier than booking date" });

                booking.RegistrationId = updatedBooking.RegistrationId;
                booking.BookingDate = updatedBooking.BookingDate;
                booking.BookingStatus = updatedBooking.BookingStatus;

                _dbcontext.Entry(booking).State = EntityState.Modified;
                await _dbcontext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "Booking updated successfully", Data = booking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ DELETE booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var booking = await _dbcontext.BookingMasters.FindAsync(id);
                if (booking == null)
                    return NotFound(new { Status = "Error", Message = "Booking not found" });

                _dbcontext.BookingMasters.Remove(booking);
                await _dbcontext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "Booking deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("CustomerBookings/{Id}")]
        public async Task<IActionResult> customerBookings(int Id)
        {
            try
            {
                var Data = await _dbcontext.BookingMasters.Where(b => b.RegistrationId == Id)
                .Select(o => new
                {
                    o.BookingDate,
                    o.BookingStatus,
                    slotes = o.Slots.Select(S => new
                    {
                        S.Rate,
                        S.Timeslot
                    }),
                    payments = o.Payments.Select(P => new
                    {
                        P.Amount,
                        P.PaymentType,
                        P.Remark,
                        P.PaymentDate
                    })
                }).ToListAsync();

                return Ok(new { Status = "OK", Result = Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }


        [HttpGet("upcomming-bookings")]
        public async Task<IActionResult> upcommingBookings()
        {
            try
            {
                var data = await _dbcontext.BookingMasters.Where(o => o.BookingDate.Date > DateTime.Today.Date)
                .Select(o => new
                {
                    Customer = o.Registration != null ? new
                    {
                        o.Registration.FullName,
                        o.Registration.ContactNo
                    } : null,
                    slotes = o.Slots.Select(S => new
                    {
                        S.Rate,
                        Slote = S.Timeslot != null ? S.Timeslot.Timename : null
                    }),
                    totalSlotes = o.Slots.Count(),
                    Total = o.Slots.Sum(S => S.Rate),
                    payments = o.Payments.Sum(P => (P != null ? P.Amount : 0)),
                    o.BookingDate,
                    o.createdAt,
                    o.RegistrationId
                }).ToListAsync();

                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }

        }

        [HttpGet("todays-bookings")]
        public async Task<IActionResult> todaysBookings()
        {
            try
            {
                var data = await _dbcontext.BookingMasters.Where(o => o.BookingDate.Date == DateTime.Today.Date)
                .Select(o => new
                {
                    Customer = o.Registration != null ? new
                    {
                        o.Registration.FullName,
                        o.Registration.ContactNo
                    } : null,
                    slotes = o.Slots.Select(S => new
                    {
                        S.Rate,
                        Slote = S.Timeslot != null ? S.Timeslot.Timename : null
                    }),
                    totalSlotes = o.Slots.Count(),
                    Total = o.Slots.Sum(S => S.Rate),
                    payments = o.Payments.Sum(P => (P != null ? P.Amount : 0)),
                    o.BookingDate,
                    o.createdAt,
                    o.RegistrationId
                }).ToListAsync();

                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        [HttpGet("complete-bookings")]
        public async Task<IActionResult> completedBookings()
        {
            try
            {
                var data = await _dbcontext.BookingMasters.Where(o => o.BookingDate.Date <= DateTime.Today.Date)
                .Select(o => new
                {
                    Customer = o.Registration != null ? new
                    {
                        o.Registration.FullName,
                        o.Registration.ContactNo
                    } : null,
                    slotes = o.Slots.Select(S => new
                    {
                        S.Rate,
                        Slote = S.Timeslot != null ? S.Timeslot.Timename : null
                    }),
                    totalSlotes = o.Slots.Count(),
                    Total = o.Slots.Sum(S => S.Rate),
                    payments = o.Payments.Sum(P => (P != null ? P.Amount : 0)),
                    o.BookingDate,
                    o.createdAt,
                    o.RegistrationId
                }).ToListAsync();

                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        [HttpGet("bookings/details/{id}")]
        public async Task<IActionResult> bookingDetails(int id)
        {
            try
            {
                var data = await _dbcontext.BookingMasters.Where(o => o.Id == id)
                .Select(o => new
                {
                    Customer = o.Registration != null ? new
                    {
                        o.Registration.FullName,
                        o.Registration.ContactNo
                    } : null,
                    slotes = o.Slots.Select(S => new
                    {
                        S.Id,
                        S.Rate,
                        Slote = S.Timeslot != null ? S.Timeslot.Timename : null
                    }).ToList(),
                    totalSlotes = o.Slots.Count(),
                    Total = o.Slots.Sum(S => S.Rate),
                    o.BookingDate,
                    o.createdAt
                }).FirstOrDefaultAsync();

                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        [HttpPost("RemoveSlotes")]
        public async Task<IActionResult> RemoveSlotes(List<int> ids)
        {
            try
            {
                var slotes = await _dbcontext.BookingSlots.Where(s => ids.Contains(s.Id)).ToListAsync();
                if (slotes == null || slotes.Count == 0)
                {
                    return NotFound(new { Status = "Error", Message = "No slots found for the given booking ID" });
                }

                _dbcontext.BookingSlots.RemoveRange(slotes);
                await _dbcontext.SaveChangesAsync();
                return Ok(new { Status = "OK", Message = "Slots removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }

        }
    }
}
