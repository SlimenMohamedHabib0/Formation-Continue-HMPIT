using FormationContinue.Data;
using FormationContinue.Dtos.AdminCourse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminCoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminCoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("courses")]
        public async Task<ActionResult<List<AdminCourseResponseDto>>> GetAll([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] string? etat)
        {
            var q = _context.Courses.AsNoTracking();

            if (categoryId.HasValue)
                q = q.Where(c => c.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(etat))
            {
                var e = etat.Trim().ToUpper();
                q = q.Where(c => c.Etat.ToUpper() == e);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(c =>
                    c.Titre.ToLower().Contains(s) ||
                    c.Description.ToLower().Contains(s) ||
                    c.MotsCles.ToLower().Contains(s));
            }

            var courses = await q
                .Select(c => new AdminCourseResponseDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Etat = c.Etat,
                    DatePublication = c.DatePublication,
                    CategoryId = c.CategoryId,
                    NomFichierPdf = c.NomFichierPdf,
                    Professors = c.CourseProfessors
                        .Select(cp => new AdminCourseProfessorDto
                        {
                            Id = cp.Professor.Id,
                            FullName = cp.Professor.FullName,
                            Email = cp.Professor.Email
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(courses);

        }


        [HttpPost("courses/{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            if (course.Etat != "PUBLISHED")
                return BadRequest("Only published courses can be unpublished.");

            course.Etat = "DRAFT";
            course.DatePublication = null;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Only draft courses can be deleted.");

            var attemptsCount = await _context.TentativesQcm.CountAsync(t => t.CourseId == id);
            if (attemptsCount > 0)
                return BadRequest("Cannot delete course: attempts exist.");

            var links = await _context.CourseProfessors
                .Where(cp => cp.CourseId == id)
                .ToListAsync();

            _context.CourseProfessors.RemoveRange(links);
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
