using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turf_Point.ApplicationContext;
using Turf_Point.Models;

namespace Turf_Point.Controllers.apis
{
    [Route("api/TimeSlot")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public TimeSlotController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ✅ GET all timeslots
        [HttpGet("List")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var timeslots = await _dbContext.timeslots.ToListAsync();
                return Ok(new { Status = "OK", Data = timeslots });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("BookTimeSlots")]
        public async Task<IActionResult> BookTimeSlots([FromBody] BookSlots model)
        {
            try
            {
                var timeslots = await _dbContext.timeslots
                    .Where(o => !(
                        _dbContext.BookingSlots.Include(c => c.BookingMaster).Where(w => w.BookingMaster.BookingDate == model.BookingDate).Select(b => b.TimeslotId).Contains(o.Id)
                    ))
                    .ToListAsync();

                if (model.BookingDate.Date == DateTime.Today)
                {
                    var now = TimeOnly.FromDateTime(DateTime.Now);

                    timeslots = timeslots
                        .Where(o => o.LastBookingTime > now)
                        .ToList();       // 👈 REQUIRED
                }

                return Ok(new { Status = "OK", Data = timeslots });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }


        // ✅ GET timeslot by Id
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var timeslot = await _dbContext.timeslots.FindAsync(id);
                if (timeslot == null)
                    return NotFound(new { Status = "Error", Message = "Timeslot not found" });

                return Ok(new { Status = "OK", Data = timeslot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ INSERT new timeslot
        [HttpPost("Save")]
        public async Task<IActionResult> Create([FromBody] Timeslot timeslot)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Result = "Invalid data",
                        Errors = ModelState
                    });
                }

                // 🔍 Check if Timename already exists (case-insensitive)
                var exists = await _dbContext.timeslots.AnyAsync(t => t.Timename.ToLower() == timeslot.Timename.ToLower());

                if (exists)
                {
                    return Conflict(new
                    {
                        Status = "Error",
                        Result = $"Timeslot with name '{timeslot.Timename}' already exists."
                    });
                }

                // ✅ Insert new timeslot
                _dbContext.timeslots.Add(timeslot);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "OK",
                    Result = "Timeslot saved successfully",
                    Data = timeslot
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Result = ex.Message
                });
            }
        }



        // ✅ UPDATE existing timeslot
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Timeslot updatedTimeslot)
        {
            try
            {
                if (id != updatedTimeslot.Id)
                    return BadRequest(new { Status = "Error", Message = "Id mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(new { Status = "Error", Message = "Invalid data", Errors = ModelState });

                var timeslot = await _dbContext.timeslots.FindAsync(id);
                if (timeslot == null)
                    return NotFound(new { Status = "Error", Message = "Timeslot not found" });

                // update fields
                timeslot.Timename = updatedTimeslot.Timename;
                timeslot.Rate = updatedTimeslot.Rate;
                timeslot.Type = updatedTimeslot.Type;
                timeslot.Status = updatedTimeslot.Status;

                _dbContext.Entry(timeslot).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "Timeslot updated successfully", Data = timeslot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ DELETE timeslot
        [HttpDelete("Remove/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var timeslot = await _dbContext.timeslots.FindAsync(id);
                if (timeslot == null)
                    return NotFound(new { Status = "Error", Message = "Timeslot not found" });

                _dbContext.timeslots.Remove(timeslot);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "Timeslot deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }


        [HttpGet("AvailableSlotes/Todays")]
        public async Task<IActionResult> todaysAvailableSlotes()
        {
            try
            {
                var timeslot = await _dbContext.timeslots.Where(o => !_dbContext.BookingSlots.Include(c => c.BookingMaster).Where(w => w.BookingMaster.BookingDate.Date == DateTime.Today.Date).Select(b => b.TimeslotId).Contains(o.Id)).ToListAsync();
                if (timeslot == null)
                {
                    return NotFound(new { Status = "Error", Result = "Timeslot not found" });
                }

                return Ok(new { Status = "OK", Result = timeslot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("AvailableSlotes")]
        public async Task<IActionResult> AvailableSlotes([FromQuery] DateTime slotDate)
        {
            try
            {
                var timeslot = await _dbContext.timeslots.Where(o => !_dbContext.BookingSlots.Include(c => c.BookingMaster).Where(w => w.BookingMaster.BookingDate.Date == slotDate.Date).Select(b => b.TimeslotId).Contains(o.Id)).ToListAsync();
                if (timeslot == null)
                {
                    return NotFound(new { Status = "Error", Result = "Timeslot not found" });
                }

                return Ok(new { Status = "OK", Result = timeslot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
