using FormationContinue.Data;
using FormationContinue.Dtos.CoTeaching;
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
    public class CoTeachingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoTeachingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("courses/{courseId:int}/co-professors")]
        public async Task<ActionResult<List<CoProfessorDto>>> GetCoProfessors(int courseId)
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            var owns = (await _context.CourseProfessors.CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == profId)) > 0;
            if (!owns) return Forbid();

            var list = await _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.CourseId == courseId)
                .Select(cp => new CoProfessorDto
                {
                    Id = cp.Professor.Id,
                    FullName = cp.Professor.FullName,
                    Email = cp.Professor.Email
                })
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("professors/search")]
        public async Task<ActionResult<List<ProfessorSearchDto>>> SearchProfessors([FromQuery] string term)
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            term = (term ?? "").Trim().ToLower();
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("term is required.");

            var list = await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == "PROFESSOR" && u.Id != profId)
                .Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term))
                .OrderBy(u => u.FullName)
                .Take(20)
                .Select(u => new ProfessorSearchDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("courses/{courseId:int}/co-professors/{otherProfessorId:int}")]
        public async Task<IActionResult> AddCoProfessor(int courseId, int otherProfessorId)
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            if (otherProfessorId == profId)
                return BadRequest("Cannot add yourself.");

            var owns = (await _context.CourseProfessors.CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == profId)) > 0;
            if (!owns) return Forbid();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Co-professors can be managed only for DRAFT courses.");

            var otherProfCount = await _context.Users.CountAsync(u => u.Id == otherProfessorId && u.Role == "PROFESSOR");
            if (otherProfCount == 0)
                return BadRequest("Professor not found.");

            var exists = (await _context.CourseProfessors.CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == otherProfessorId)) > 0;
            if (exists)
                return BadRequest("Professor already assigned to this course.");

            _context.CourseProfessors.Add(new CourseProfessor
            {
                CourseId = courseId,
                ProfessorId = otherProfessorId
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("courses/{courseId:int}/co-professors/{otherProfessorId:int}")]
        public async Task<IActionResult> RemoveCoProfessor(int courseId, int otherProfessorId)
        {
            var profIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profIdStr == null || !int.TryParse(profIdStr, out var profId))
                return Unauthorized();

            var owns = (await _context.CourseProfessors.CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == profId)) > 0;
            if (!owns) return Forbid();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Co-professors can be managed only for DRAFT courses.");

            var link = await _context.CourseProfessors
                .FirstOrDefaultAsync(cp => cp.CourseId == courseId && cp.ProfessorId == otherProfessorId);

            if (link == null)
                return NotFound();

            var profCount = await _context.CourseProfessors.CountAsync(cp => cp.CourseId == courseId);
            if (profCount <= 1)
                return BadRequest("Cannot remove the last professor from a course.");

            _context.CourseProfessors.Remove(link);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
