using FormationContinue.Data;
using FormationContinue.Dtos.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class ProgressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("courses/{courseId}/progress")]
        public async Task<IActionResult> UpdateProgress(int courseId, [FromQuery] int page, [FromQuery] int totalPages)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            if (page <= 0)
                return BadRequest("Invalid page.");
            if (totalPages <= 0)
                return BadRequest("Invalid totalPages.");

            if (page > totalPages)
                page = totalPages;

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userIdInt);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            if (enrollment.Statut != "ACCEPTEE")
                return BadRequest("Enrollment not accepted.");

            var progress = await _context.Progress
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id);

            if (progress == null)
                return NotFound("Progress not found.");

          
            if (progress.DernierePageAtteinte < page)
                progress.DernierePageAtteinte = page;

            if (progress.DateCompletion == null && progress.DernierePageAtteinte >= totalPages)
                progress.DateCompletion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("courses/{courseId}/progress")]
        public async Task<ActionResult<ProgressResponseDto>> GetProgress(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userIdInt);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            var progress = await _context.Progress
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id);

            if (progress == null)
                return NotFound("Progress not found.");

            return Ok(new ProgressResponseDto
            {
                CourseId = courseId,
                EnrollmentId = enrollment.Id,
                DernierePageAtteinte = progress.DernierePageAtteinte,
                DateCompletion = progress.DateCompletion
            });
        }
    }
}
