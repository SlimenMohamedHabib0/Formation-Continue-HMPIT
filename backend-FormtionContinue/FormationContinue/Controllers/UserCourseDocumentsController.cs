using FormationContinue.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/user-courses")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserCourseDocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserCourseDocumentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{courseId:int}/pdf")]
        public async Task<IActionResult> GetCoursePdf(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.CourseId == courseId &&
                    e.UserId == userIdInt &&
                    e.Statut == "ACCEPTEE");

            if (enrollment == null)
                return Forbid();

           var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.Id == courseId &&
                    c.Etat == "PUBLISHED");

            if (course == null)
                return NotFound("Course not available.");

            if (course.ContenuPdf == null || course.ContenuPdf.Length == 0)
                return NotFound("PDF not found.");

            return File(
                course.ContenuPdf,
                "application/pdf",
                course.NomFichierPdf ?? "course.pdf",
                enableRangeProcessing: true
            );
        }
    }
}
