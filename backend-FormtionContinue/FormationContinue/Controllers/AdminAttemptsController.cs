using FormationContinue.Data;
using FormationContinue.Dtos.AdminAttempt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminAttemptsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminAttemptsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("attempts")]
        public async Task<ActionResult<List<AdminAttemptResponseDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? statut,
            [FromQuery] int? courseId,
            [FromQuery] int? categoryId,
            [FromQuery] int? userId,
            [FromQuery] int? professorId)
        {
            var q = _context.TentativesQcm.AsNoTracking();

            if (courseId.HasValue)
                q = q.Where(t => t.CourseId == courseId.Value);

            if (categoryId.HasValue)
                q = q.Where(t => t.Course.CategoryId == categoryId.Value);

            if (userId.HasValue)
                q = q.Where(t => t.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var st = statut.Trim().ToUpper();
                q = q.Where(t => t.StatutTentative.ToUpper() == st);
            }

            if (professorId.HasValue)
            {
                var pid = professorId.Value;
                q = q.Where(t =>
                    _context.CourseProfessors.Count(cp => cp.CourseId == t.CourseId && cp.ProfessorId == pid) > 0);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(t =>
                    t.Course.Titre.ToLower().Contains(s) ||
                    t.User.FullName.ToLower().Contains(s) ||
                    t.User.Email.ToLower().Contains(s));
            }

            var list = await q
                .OrderByDescending(t => t.DateTentative)
                .Select(t => new AdminAttemptResponseDto
                {
                    TentativeId = t.Id,
                    CourseId = t.CourseId,
                    CourseTitre = t.Course.Titre,

                    UserId = t.UserId,
                    UserFullName = t.User.FullName,
                    UserEmail = t.User.Email,

                    DateTentative = t.DateTentative,
                    NoteSur20 = t.NoteSur20,
                    StatutTentative = t.StatutTentative
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
