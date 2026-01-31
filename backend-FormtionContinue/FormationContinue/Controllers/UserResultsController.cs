using FormationContinue.Data;
using FormationContinue.Dtos.UserResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserResultsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserResultsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("attempts")]
        public async Task<ActionResult<List<UserAttemptDto>>> GetMyAttempts(
            [FromQuery] string? search,
            [FromQuery] int? courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var q = _context.TentativesQcm
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            if (courseId.HasValue)
                q = q.Where(t => t.CourseId == courseId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(t => t.Course.Titre.ToLower().Contains(s));
            }

            var list = await q
                .OrderByDescending(t => t.DateTentative)
                .Select(t => new UserAttemptDto
                {
                    TentativeId = t.Id,
                    CourseId = t.CourseId,
                    CourseTitre = t.Course.Titre,
                    DateTentative = t.DateTentative,
                    NoteSur20 = t.NoteSur20,
                    StatutTentative = t.StatutTentative
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("results/summary")]
        public async Task<ActionResult<UserResultsSummaryDto>> GetSummary()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var nbEnrolled = await _context.Enrollments.CountAsync(e => e.UserId == userId);
            var nbPending = await _context.Enrollments.CountAsync(e => e.UserId == userId && e.Statut == "PENDING");
            var nbAccepted = await _context.Enrollments.CountAsync(e => e.UserId == userId && e.Statut == "ACCEPTEE");
            var nbRefused = await _context.Enrollments.CountAsync(e => e.UserId == userId && e.Statut == "REFUSEE");

            var nbCompleted = await _context.Progress.CountAsync(p =>
                p.Enrollment.UserId == userId &&
                p.DateCompletion != null);

            var nbPassed = await _context.TentativesQcm.CountAsync(t => t.UserId == userId && t.StatutTentative == "REUSSI");
            var nbFailed = await _context.TentativesQcm.CountAsync(t => t.UserId == userId && t.StatutTentative == "ECHOUE");

            var avgNote = await _context.TentativesQcm
                .Where(t => t.UserId == userId)
                .Select(t => (double?)t.NoteSur20)
                .AverageAsync();

            var lastTest = await _context.TentativesQcm
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DateTentative)
                .Select(t => new { t.NoteSur20, t.StatutTentative })
                .FirstOrDefaultAsync();

            return Ok(new UserResultsSummaryDto
            {
                NbCoursesEnrolled = nbEnrolled,
                NbCoursesPending = nbPending,
                NbCoursesAccepted = nbAccepted,
                NbCoursesRefused = nbRefused,
                NbCoursesCompleted = nbCompleted,

                NbPassedTests = nbPassed,
                NbFailedTests = nbFailed,
                AverageNote = avgNote,
                LastTestNote = lastTest?.NoteSur20,
                LastTestStatus = lastTest?.StatutTentative
            });
        }

    }
}
