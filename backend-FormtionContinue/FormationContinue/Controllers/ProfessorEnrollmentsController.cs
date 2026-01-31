using FormationContinue.Data;
using FormationContinue.Dtos.ProfessorEnrollment;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/professor")]
    [ApiController]
    [Authorize(Roles = "PROFESSOR")]
    public class ProfessorEnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfessorEnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("enrollments")]
        public async Task<ActionResult<List<ProfessorEnrollmentResponseDto>>> GetMyCourseEnrollments([FromQuery] string? statut, [FromQuery] string? search)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var q = _context.Enrollments
                .AsNoTracking()
                .Where(e => _context.CourseProfessors.Count(cp => cp.CourseId == e.CourseId && cp.ProfessorId == professorIdInt) > 0);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var s = statut.Trim().ToUpper();
                q = q.Where(e => e.Statut.ToUpper() == s);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(e =>
                    e.Course.Titre.ToLower().Contains(s) ||
                    e.User.FullName.ToLower().Contains(s) ||
                    e.User.Email.ToLower().Contains(s));
            }


            var list = await q
                .OrderByDescending(e => e.DateDemande)
                .Select(e => new ProfessorEnrollmentResponseDto
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitre = e.Course.Titre,
                    UserId = e.UserId,
                    UserFullName = e.User.FullName,
                    UserEmail = e.User.Email,
                    Statut = e.Statut,
                    DateDemande = e.DateDemande,
                    DateDecision = e.DateDecision
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("enrollments/{id}/accept")]
        public async Task<IActionResult> Accept(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == id);
            if (enrollment == null)
                return NotFound();

            var ownsCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == enrollment.CourseId && cp.ProfessorId == professorIdInt);

            if (ownsCount == 0)
                return Forbid();

            if (enrollment.Statut != "PENDING")
                return BadRequest("Only pending enrollments can be accepted.");

            enrollment.Statut = "ACCEPTEE";
            enrollment.DateDecision = DateTime.UtcNow;
            enrollment.DecisionProfessorId = professorIdInt;

            var progressCount = await _context.Progress.CountAsync(p => p.EnrollmentId == enrollment.Id);
            if (progressCount == 0)
            {
                _context.Progress.Add(new CourseProgress
                {
                    EnrollmentId = enrollment.Id,
                    DateCompletion = null,
                    DernierePageAtteinte = 0
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("enrollments/{id}/refuse")]
        public async Task<IActionResult> Refuse(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == id);
            if (enrollment == null)
                return NotFound();

            var ownsCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == enrollment.CourseId && cp.ProfessorId == professorIdInt);

            if (ownsCount == 0)
                return Forbid();

            if (enrollment.Statut != "PENDING")
                return BadRequest("Only pending enrollments can be refused.");

            enrollment.Statut = "REFUSEE";
            enrollment.DateDecision = DateTime.UtcNow;
            enrollment.DecisionProfessorId = professorIdInt;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
