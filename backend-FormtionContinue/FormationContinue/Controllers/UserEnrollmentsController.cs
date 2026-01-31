using FormationContinue.Data;
using FormationContinue.Dtos.Enrollment;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserEnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserEnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("courses/{courseId}/enrollments/request")]
        public async Task<IActionResult> RequestEnrollment(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null || course.Etat != "PUBLISHED")
                return NotFound("Course not available.");

            var existingCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == courseId && e.UserId == userIdInt);

            if (existingCount > 0)
                return BadRequest("Enrollment already exists.");

            _context.Enrollments.Add(new Enrollment
            {
                CourseId = courseId,
                UserId = userIdInt,
                Statut = "PENDING",
                DateDemande = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("my/enrollments")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetMyEnrollments([FromQuery] string? search, [FromQuery] string? statut)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var q = _context.Enrollments
    .AsNoTracking()
    .Where(e => e.UserId == userIdInt);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var st = statut.Trim().ToUpper();
                q = q.Where(e => e.Statut.ToUpper() == st);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(e =>
                    e.Course.Titre.ToLower().Contains(s) ||
                    e.Course.MotsCles.ToLower().Contains(s));
            }

            var list = await q
                .OrderByDescending(e => e.DateDemande)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitre = e.Course.Titre,
                    Statut = e.Statut,
                    DateDemande = e.DateDemande,
                    DateDecision = e.DateDecision
                })
                .ToListAsync();

            return Ok(list);

        }
    }
}
