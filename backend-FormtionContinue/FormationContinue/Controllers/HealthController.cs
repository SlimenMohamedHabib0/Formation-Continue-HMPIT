using FormationContinue.Data;
using Microsoft.AspNetCore.Mvc;

namespace FormationContinue.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }
         
        [HttpGet("db")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();

                if (canConnect)
                {
                    return Ok("Database connection OK");
                }

                return StatusCode(500, "Database connection FAILED");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
}
