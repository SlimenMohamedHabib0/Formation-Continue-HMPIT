using FormationContinue.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FormationContinue.Dtos.Category;
using Microsoft.EntityFrameworkCore;
using FormationContinue.Models;


namespace FormationContinue.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]


    public class CategoryController : ControllerBase
    {

        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {

            _context = context;
        }
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetAll([FromQuery] string? search)
        {
            var q = _context.Categories.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(c => c.Libelle.ToLower().Contains(s));
            }

            var categories = await q
                .Select(c => new CategoryResponseDto { Id = c.Id, Libelle = c.Libelle })
                .OrderBy(c => c.Libelle)
                .ToListAsync();

            return Ok(categories);


        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
        {
           
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (category == null )
            {
                return NotFound();
            }
            var CatDto = new CategoryResponseDto
            {
                Id = category.Id,
                Libelle = category.Libelle,
            };
            return Ok(CatDto);
        }



        [HttpPut("categories/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);
            if (category == null)
                return NotFound();

            var newLibelle = (dto.Libelle ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newLibelle))
                return BadRequest("Libelle is required.");

            var normalized = newLibelle.ToLower();

            var existsCount = await _context.Categories
     .CountAsync(c => c.Id != id && c.Libelle.ToLower() == normalized);

            if (existsCount > 0)
                return BadRequest("Category Already exist");



            category.Libelle = newLibelle;
            await _context.SaveChangesAsync();

            return NoContent();
        }




        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {

            var category = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var hasCourses = (await _context.Courses.CountAsync(c => c.CategoryId == id)) > 0;
            if (hasCourses)
            {
                return BadRequest("Cannot delete category: it has courses.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
            
        }


    } 
}
