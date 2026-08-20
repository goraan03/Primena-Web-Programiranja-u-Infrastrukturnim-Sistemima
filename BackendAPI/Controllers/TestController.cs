using BackendAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TestController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var travelCount = await _db.Travels.CountAsync();
                var destinationCount = await _db.Destinations.CountAsync();
                var activityCount = await _db.Activities.CountAsync();
                var expenseCount = await _db.Expenses.CountAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Database connection successful!",
                    Tables = new
                    {
                        Travels = travelCount,
                        Destinations = destinationCount,
                        Activities = activityCount,
                        Expenses = expenseCount
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message,
                    InnerErrorType = ex.InnerException?.GetType().FullName,
                    StackTrace = ex.StackTrace
                });
            }
        }
    }
}