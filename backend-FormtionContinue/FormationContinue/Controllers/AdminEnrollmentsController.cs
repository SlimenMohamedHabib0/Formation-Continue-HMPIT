using FormationContinue.Data;
using FormationContinue.Dtos.AdminEnrollment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminEnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminEnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("enrollments")]
        public async Task<ActionResult<List<AdminEnrollmentResponseDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? statut,
            [FromQuery] int? courseId,
            [FromQuery] int? categoryId,
            [FromQuery] int? userId,
            [FromQuery] int? professorId)
        {
            var q = _context.Enrollments.AsNoTracking();

            if (courseId.HasValue)
                q = q.Where(e => e.CourseId == courseId.Value);

            if (categoryId.HasValue)
                q = q.Where(e => e.Course.CategoryId == categoryId.Value);

            if (userId.HasValue)
                q = q.Where(e => e.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var st = statut.Trim().ToUpper();
                q = q.Where(e => e.Statut.ToUpper() == st);
            }

            if (professorId.HasValue)
            {
                var pid = professorId.Value;
                q = q.Where(e =>
                    _context.CourseProfessors.Count(cp => cp.CourseId == e.CourseId && cp.ProfessorId == pid) > 0);
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
                .Select(e => new AdminEnrollmentResponseDto
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitre = e.Course.Titre,
                    CategoryId = e.Course.CategoryId,

                    UserId = e.UserId,
                    UserFullName = e.User.FullName,
                    UserEmail = e.User.Email,

                    Statut = e.Statut,
                    DateDemande = e.DateDemande,
                    DateDecision = e.DateDecision,

                    DecisionProfessorId = e.DecisionProfessorId,
                    DecisionProfessorFullName = _context.Users
                        .Where(u => e.DecisionProfessorId != null && u.Id == e.DecisionProfessorId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    DecisionProfessorEmail = _context.Users
                        .Where(u => e.DecisionProfessorId != null && u.Id == e.DecisionProfessorId)
                        .Select(u => u.Email)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
