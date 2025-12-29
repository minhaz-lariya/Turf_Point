using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turf_Point.ApplicationContext;
using Turf_Point.Models;

namespace Turf_Point.Controllers.apis
{
    [Route("api/Registration")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        
        public RegistrationController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _dbContext.registrations.ToListAsync();
                return Ok(new { Status = "OK", Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _dbContext.registrations.FindAsync(id);
                if (user == null)
                    return NotFound(new { Status = "Error", Message = "User not found" });

                return Ok(new { Status = "OK", Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Create([FromBody] Registration registration)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Status = "Error", Result = "Invalid data", Errors = ModelState });

                var errors = new List<string>();

                // Check unique ContactNo
                if (await _dbContext.registrations.AnyAsync(r => r.ContactNo == registration.ContactNo))
                    errors.Add("Contact number already exists");

                // Check unique UserName (Email)
                if (await _dbContext.registrations.AnyAsync(r => r.UserName == registration.UserName))
                    errors.Add("Email already exists");

                if (errors.Count > 0)
                    return BadRequest(new { Status = "Error", Result = errors });

                registration.Created_At = DateTime.Now;
                _dbContext.registrations.Add(registration);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Status = "OK", Result = "User registered successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Result = ex.Message });
            }
        }

        // ✅ Update
        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] Registration updatedUser)
        {
            try
            {
                if (id != updatedUser.Id)
                    return BadRequest(new { Status = "Error", Message = "Id mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(new { Status = "Error", Message = "Invalid data", Errors = ModelState });

                var existingUser = await _dbContext.registrations.FindAsync(id);
                if (existingUser == null)
                    return NotFound(new { Status = "Error", Message = "User not found" });

                // Check unique ContactNo (excluding current user)
                if (await _dbContext.registrations.AnyAsync(r => r.ContactNo == updatedUser.ContactNo && r.Id != id))
                    return BadRequest(new { Status = "Error", Message = "Contact number already exists" });

                // Check unique Email (excluding current user)
                if (await _dbContext.registrations.AnyAsync(r => r.UserName == updatedUser.UserName && r.Id != id))
                    return BadRequest(new { Status = "Error", Message = "Email already exists" });

                existingUser.FullName = updatedUser.FullName;
                existingUser.ContactNo = updatedUser.ContactNo;
                existingUser.UserName = updatedUser.UserName;
                existingUser.Password = updatedUser.Password; // ⚠️ Hash in real-world apps

                _dbContext.Entry(existingUser).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "User updated successfully", Data = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _dbContext.registrations.FindAsync(id);
                if (user == null)
                    return NotFound(new { Status = "Error", Message = "User not found" });

                _dbContext.registrations.Remove(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Status = "OK", Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("Authentication")]
        public async Task<IActionResult> Login([FromBody] Authentication loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
                    return BadRequest(new { Status = "Error", Message = "Username and password are required" });

                var user = await _dbContext.registrations.FirstOrDefaultAsync(u => u.ContactNo == loginRequest.UserName && u.Password == loginRequest.Password);

                if (user == null)
                    return Unauthorized(new { Status = "Error", Message = "Invalid username or password" });

                return Ok(new
                {
                    Status = "OK",
                    Message = "Login successful",
                    Data = new
                    {
                        user.Id,
                        user.FullName,
                        user.UserName,
                        user.ContactNo
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
