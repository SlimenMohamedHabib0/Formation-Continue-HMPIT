using FormationContinue.Data;
using FormationContinue.Dtos.AdminStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            var nbUsers = await _context.Users.CountAsync(u => u.Role == "USER");
            var nbProfessors = await _context.Users.CountAsync(u => u.Role == "PROFESSOR");
            var nbAdmins = await _context.Users.CountAsync(u => u.Role == "ADMIN");

            var nbCategories = await _context.Categories.CountAsync();
            var nbCoursesTotal = await _context.Courses.CountAsync();
            var nbCoursesDraft = await _context.Courses.CountAsync(c => c.Etat == "DRAFT");
            var nbCoursesPublished = await _context.Courses.CountAsync(c => c.Etat == "PUBLISHED");

            var nbEnrollTotal = await _context.Enrollments.CountAsync();
            var nbEnrollPending = await _context.Enrollments.CountAsync(e => e.Statut == "PENDING");
            var nbEnrollAccepted = await _context.Enrollments.CountAsync(e => e.Statut == "ACCEPTEE");
            var nbEnrollRefused = await _context.Enrollments.CountAsync(e => e.Statut == "REFUSEE");

            var nbAttempts = await _context.TentativesQcm.CountAsync();
            var nbPassed = await _context.TentativesQcm.CountAsync(t => t.StatutTentative == "REUSSI");
            var nbFailed = await _context.TentativesQcm.CountAsync(t => t.StatutTentative == "ECHOUE");

            var avgNote = await _context.TentativesQcm
                .Select(t => (double?)t.NoteSur20)
                .AverageAsync();

            var successRate = nbAttempts == 0 ? 0 : (double)nbPassed * 100.0 / nbAttempts;

            var topCoursesByEnrollments = await _context.Enrollments
                .AsNoTracking()
                .GroupBy(e => new { e.CourseId, e.Course.Titre })
                .Select(g => new CountItemDto
                {
                    Id = g.Key.CourseId,
                    Label = g.Key.Titre,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            var topCategoriesByCourses = await _context.Courses
                .AsNoTracking()
                .GroupBy(c => new { c.CategoryId, c.Category.Libelle })
                .Select(g => new CountItemDto
                {
                    Id = g.Key.CategoryId,
                    Label = g.Key.Libelle,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            var topCategoriesByEnrollments = await _context.Enrollments
                .AsNoTracking()
                .GroupBy(e => new { e.Course.CategoryId, e.Course.Category.Libelle })
                .Select(g => new CountItemDto
                {
                    Id = g.Key.CategoryId,
                    Label = g.Key.Libelle,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            return Ok(new AdminDashboardDto
            {
                NbUsers = nbUsers,
                NbProfessors = nbProfessors,
                NbAdmins = nbAdmins,

                NbCategories = nbCategories,
                NbCoursesTotal = nbCoursesTotal,
                NbCoursesDraft = nbCoursesDraft,
                NbCoursesPublished = nbCoursesPublished,

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
                TopCategoriesByCourses = topCategoriesByCourses,
                TopCategoriesByEnrollments = topCategoriesByEnrollments
            });
        }
    }
}
