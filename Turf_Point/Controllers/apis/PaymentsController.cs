using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turf_Point.ApplicationContext;
using Turf_Point.Models;

namespace Turf_Point.Controllers.apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        public PaymentsController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> savePayment(PaymentMaster model)
        {
            try
            {
                _dbContext.paymentMasters.Add(model);
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = "OK", Result = "Successfully Saved" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        public async Task<IActionResult> paymentList()
        {
            try
            {
                var data = await _dbContext.paymentMasters
                .Select(o => new
                {
                    o.BookingMasterId,
                    o.Amount,
                    o.Remark,
                    o.PaymentDate,
                    o.PaymentType,
                    customer = o.BookingMaster.Registration != null
                        ? new
                        {
                            o.BookingMaster.Registration.FullName,
                            o.BookingMaster.Registration.ContactNo,
                            o.BookingMaster.Registration.Id
                        } : null,
                    booking = o.BookingMaster != null
                        ? new
                        {
                            count = o.BookingMaster.Slots.Count(),
                            totalCost = o.BookingMaster.Slots.Sum(s => s.Rate),
                        } : null
                }).ToListAsync();

                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        [HttpGet("paymentList/{bookingId}")]
        public async Task<IActionResult> paymentList(int bookingId)
        {
            try
            {
                var data = await _dbContext.paymentMasters.Where(p => p.BookingMasterId == bookingId).ToListAsync();
                return Ok(new { Status = "OK", Result = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }


    }
}
