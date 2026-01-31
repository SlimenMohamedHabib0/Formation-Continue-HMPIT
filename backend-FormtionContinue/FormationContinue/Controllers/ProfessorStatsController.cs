using FormationContinue.Data;
using FormationContinue.Dtos.ProfessorStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/professor")]
    [ApiController]
    [Authorize(Roles = "PROFESSOR")]
    public class ProfessorStatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfessorStatsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("attempts")]
        public async Task<ActionResult<List<ProfessorAttemptDto>>> GetAttempts(
            [FromQuery] int? courseId,
            [FromQuery] string? statut,
            [FromQuery] string? search)
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            var q = _context.TentativesQcm
                .AsNoTracking()
                .Where(t =>
                    _context.CourseProfessors.Count(cp => cp.CourseId == t.CourseId && cp.ProfessorId == profId) > 0
                );

            if (courseId.HasValue)
                q = q.Where(t => t.CourseId == courseId.Value);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var st = statut.Trim().ToUpper();
                q = q.Where(t => t.StatutTentative.ToUpper() == st);
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
                .Select(t => new ProfessorAttemptDto
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

        [HttpGet("dashboard")]
        public async Task<ActionResult<ProfessorDashboardDto>> GetDashboard()
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            var myCourseIds = await _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.ProfessorId == profId)
                .Select(cp => cp.CourseId)
                .Distinct()
                .ToListAsync();

            var nbMyCoursesTotal = myCourseIds.Count;

            var nbDraft = await _context.Courses.CountAsync(c => myCourseIds.Contains(c.Id) && c.Etat == "DRAFT");
            var nbPublished = await _context.Courses.CountAsync(c => myCourseIds.Contains(c.Id) && c.Etat == "PUBLISHED");

            var nbEnrollTotal = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId));
            var nbEnrollPending = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "PENDING");
            var nbEnrollAccepted = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "ACCEPTEE");
            var nbEnrollRefused = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "REFUSEE");

            var nbAttempts = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId));
            var nbPassed = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId) && t.StatutTentative == "REUSSI");
            var nbFailed = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId) && t.StatutTentative == "ECHOUE");

            var avgNote = await _context.TentativesQcm
                .Where(t => myCourseIds.Contains(t.CourseId))
                .Select(t => (double?)t.NoteSur20)
                .AverageAsync();

            var successRate = nbAttempts == 0 ? 0 : (double)nbPassed * 100.0 / nbAttempts;

            var topCoursesByEnrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => myCourseIds.Contains(e.CourseId))
                .GroupBy(e => new { e.CourseId, e.Course.Titre })
                .Select(g => new CourseCountItemDto
                {
                    CourseId = g.Key.CourseId,
                    CourseTitre = g.Key.Titre,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            var successByCourseRaw = await _context.TentativesQcm
                .AsNoTracking()
                .Where(t => myCourseIds.Contains(t.CourseId))
                .GroupBy(t => new { t.CourseId, t.Course.Titre })
                .Select(g => new
                {
                    g.Key.CourseId,
                    g.Key.Titre,
                    Attempts = g.Count(),
                    Passed = g.Count(x => x.StatutTentative == "REUSSI"),
                    Failed = g.Count(x => x.StatutTentative == "ECHOUE")
                })
                .ToListAsync();

            var successRateByCourse = successByCourseRaw
                .Select(x => new CourseSuccessItemDto
                {
                    CourseId = x.CourseId,
                    CourseTitre = x.Titre,
                    Attempts = x.Attempts,
                    Passed = x.Passed,
                    Failed = x.Failed,
                    SuccessRatePercent = x.Attempts == 0 ? 0 : (double)x.Passed * 100.0 / x.Attempts
                })
                .OrderByDescending(x => x.Attempts)
                .Take(10)
                .ToList();

            return Ok(new ProfessorDashboardDto
            {
                NbMyCoursesTotal = nbMyCoursesTotal,
                NbMyCoursesDraft = nbDraft,
                NbMyCoursesPublished = nbPublished,

                NbEnrollmentsTotal = nbEnrollTotal,
                NbEnrollmentsPending = nbEnrollPending,
                NbEnrollmentsAccepted = nbEnrollAccepted,
                NbEnrollmentsRefused = nbEnrollRefused,

                NbAttemptsTotal = nbAttempts,
                NbAttemptsPassed = nbPassed,
                NbAttemptsFailed = nbFailed,
                AverageNote = avgNote,
                SuccessRatePercent = successRate,

                TopCoursesByEnrollments = topCoursesByEnrollments,
                SuccessRateByCourse = successRateByCourse
            });
        }
    }
}
