using FormationContinue.Data;
using FormationContinue.Dtos.UserCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/user-courses")]
    [ApiController]
    [Authorize] 
    public class UserCoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserCoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserCourseListItemDto>>> GetPublished(
            [FromQuery] string? search,
            [FromQuery] int? categoryId)
        {
            var q = _context.Courses
                .AsNoTracking()
                .Where(c => c.Etat == "PUBLISHED");

            if (categoryId.HasValue)
                q = q.Where(c => c.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(c =>
                    c.Titre.ToLower().Contains(s) ||
                    c.Description.ToLower().Contains(s) ||
                    c.MotsCles.ToLower().Contains(s) ||
                    c.Category.Libelle.ToLower().Contains(s));
            }

            var list = await q
                .OrderByDescending(c => c.DatePublication)
                .Select(c => new UserCourseListItemDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    MotsCles = c.MotsCles,
                    DatePublication = c.DatePublication,
                    CategoryId = c.CategoryId,
                    CategoryLibelle = c.Category.Libelle,
                    NomFichierPdf = c.NomFichierPdf
                })
                .ToListAsync();

            return Ok(list);
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserCourseDetailsDto>> GetPublishedById(int id)
        {
            var dto = await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == id && c.Etat == "PUBLISHED")
                .Select(c => new UserCourseDetailsDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    MotsCles = c.MotsCles,
                    DatePublication = c.DatePublication,
                    CategoryId = c.CategoryId,
                    CategoryLibelle = c.Category.Libelle,
                    NomFichierPdf = c.NomFichierPdf,
                    Professors = c.CourseProfessors
                        .Select(cp => new UserCourseProfessorDto
                        {
                            Id = cp.Professor.Id,
                            FullName = cp.Professor.FullName,
                            Email = cp.Professor.Email
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound("Course not available.");

            return Ok(dto);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "USER")]
        public async Task<ActionResult<List<MyCourseDto>>> GetMine(
            [FromQuery] string? search,
            [FromQuery] string? statut)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var q = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.UserId == userIdInt);

            if (!string.IsNullOrWhiteSpace(statut))
            {
                var st = statut.Trim().ToUpper();
                q = q.Where(e => e.Statut.ToUpper() == st);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(e =>
                    e.Course.Titre.ToLower().Contains(s) ||
                    e.Course.Description.ToLower().Contains(s) ||
                    e.Course.MotsCles.ToLower().Contains(s) ||
                    e.Course.Category.Libelle.ToLower().Contains(s));
            }

            var list = await q
                .OrderByDescending(e => e.DateDemande)
                .Select(e => new MyCourseDto
                {
                    CourseId = e.CourseId,
                    CourseTitre = e.Course.Titre,
                    CourseEtat = e.Course.Etat,

                    CategoryId = e.Course.CategoryId,
                    CategoryLibelle = e.Course.Category.Libelle,

                    EnrollmentId = e.Id,
                    EnrollmentStatut = e.Statut,
                    DateDemande = e.DateDemande,
                    DateDecision = e.DateDecision,

                    DernierePageAtteinte = _context.Progress
                        .Where(p => p.EnrollmentId == e.Id)
                        .Select(p => p.DernierePageAtteinte)
                        .FirstOrDefault(),

                    DateCompletion = _context.Progress
                        .Where(p => p.EnrollmentId == e.Id)
                        .Select(p => p.DateCompletion)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
