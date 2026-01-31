using FormationContinue.Data;
using FormationContinue.Dtos.Course;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FormationContinue.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "PROFESSOR")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CourseController(AppDbContext context)
        { 
            _context = context;
        }

        [HttpGet("courses")]
        public async Task<ActionResult<List<CourseResponseDto>>> GetAll([FromQuery] string? search, [FromQuery] int? categoryId)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var q = _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.ProfessorId == professorIdInt);

            if (categoryId.HasValue)
                q = q.Where(cp => cp.Course.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(cp =>
                    cp.Course.Titre.ToLower().Contains(s) ||
                    cp.Course.Description.ToLower().Contains(s) ||
                    cp.Course.MotsCles.ToLower().Contains(s));
            }

            var courses = await q
                .OrderByDescending(cp => cp.Course.Id)
                .Select(cp => new CourseResponseDto
                {
                    Id = cp.Course.Id,
                    Titre = cp.Course.Titre,
                    Description = cp.Course.Description,
                    MotsCles = cp.Course.MotsCles,
                    Etat = cp.Course.Etat,
                    DatePublication = cp.Course.DatePublication,
                    NomFichierPdf = cp.Course.NomFichierPdf,
                    CategoryId = cp.Course.CategoryId
                })
                .ToListAsync();

            return Ok(courses);
        }


        [HttpGet("courses/{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetById(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(professorId, out var professorIdInt))
            {
                return Unauthorized();
            }

            var course = await _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }
            var CourseDto = new CourseResponseDto
            {
                Id = course.Id,
                Titre = course.Titre,
                Description = course.Description,
                MotsCles =course.MotsCles,
                Etat = course.Etat,
                DatePublication=course.DatePublication,
                NomFichierPdf = course.NomFichierPdf,
                CategoryId = course.CategoryId

            };
            return Ok(CourseDto);
        }
        [HttpPost("courses")]
        public async Task<ActionResult<CourseResponseDto>> Create(CourseCreateDto dto)
        {
            var categoryName = (dto.CategoryName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(categoryName))
                return BadRequest("CategoryName is required.");

            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var normalized = categoryName.ToLower();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Libelle.ToLower() == normalized);

            if (category == null)
            {
                category = new Category { Libelle = categoryName };
                _context.Categories.Add(category);

                   await _context.SaveChangesAsync();
                
            }

            var course = new Course
            {
                Titre = dto.Titre.Trim(),
                Description = dto.Description,
                MotsCles = dto.MotsCles.Trim(),
                CategoryId = category.Id,
                Etat = "DRAFT",
                DatePublication = null,
                NomFichierPdf = null,
                ContenuPdf = null,
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            _context.CourseProfessors.Add(new CourseProfessor
            {
                CourseId = course.Id,
                ProfessorId = professorIdInt
            });
            await _context.SaveChangesAsync();

            var courseDto = new CourseResponseDto
            {
                Id = course.Id,
                Titre = course.Titre,
                Description = course.Description,
                MotsCles = course.MotsCles,
                Etat = course.Etat,
                DatePublication = course.DatePublication,
                NomFichierPdf = course.NomFichierPdf,
                CategoryId = course.CategoryId
            };

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, courseDto);
        }

        [HttpPut("courses/{id}")]
        public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
        {
            var categoryName = (dto.CategoryName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(categoryName))
                return BadRequest("CategoryName is required.");

            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Only draft courses can be updated.");

            var normalized = categoryName.ToLower();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Libelle.ToLower() == normalized);

            if (category == null)
            {
                category = new Category { Libelle = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            course.Titre = dto.Titre.Trim();
            course.Description = dto.Description;
            course.MotsCles = dto.MotsCles.Trim();
            course.CategoryId = category.Id;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("courses/{id}/attach-pdf")]
        public async Task<IActionResult> AttachPdf(int id, IFormFile pdf)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(professorId, out var professorIdInt))
            {
                return Unauthorized();
            }

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            if (course.Etat != "DRAFT")
            {
                return BadRequest("PDF can only be attached to a draft course.");
            }

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("PDF file is required.");
            }

            using var ms = new MemoryStream();
            await pdf.CopyToAsync(ms);

            course.NomFichierPdf = pdf.FileName;
            course.ContenuPdf = ms.ToArray();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("courses/{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(professorId, out var professorIdInt))
            {
                return Unauthorized();
            }

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            if (course.Etat != "DRAFT")
            {
                return BadRequest("Only draft courses can be published.");
            }

            if (course.ContenuPdf == null || course.ContenuPdf.Length == 0 || string.IsNullOrWhiteSpace(course.NomFichierPdf))
            {
                return BadRequest("You must attach a PDF before publishing.");
            }

            course.Etat = "PUBLISHED";
            course.DatePublication = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("courses/{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(professorId, out var professorIdInt))
            {
                return Unauthorized();
            }

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            if (course.Etat != "PUBLISHED")
            {
                return BadRequest("Only published courses can be unpublished.");
            }

            course.Etat = "DRAFT";
            course.DatePublication = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Only draft courses can be deleted. Unpublish first.");

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

        [HttpGet("courses/{id}/pdf")]
        public async Task<IActionResult> GetPdf(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var course = await _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            if (course.ContenuPdf == null || course.ContenuPdf.Length == 0)
                return NotFound();

            return File(course.ContenuPdf, "application/pdf");
        }

    }
}
