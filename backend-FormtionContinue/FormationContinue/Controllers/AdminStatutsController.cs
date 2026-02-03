using FormationContinue.Data;
using FormationContinue.Dtos.Statut;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminStatutsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminStatutsController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("statuts")]
        public async Task<ActionResult<List<StatutResponseDto>>> GetAll([FromQuery] string? search)
        {
            var q = _context.Statuts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x => x.Libelle.ToLower().Contains(s));
            }

            var list = await q
                .OrderBy(x => x.Libelle)
                .Select(x => new StatutResponseDto
                {
                    Id = x.Id,
                    Libelle = x.Libelle
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("statuts")]
        public async Task<ActionResult<StatutResponseDto>> Create(StatutCreateDto dto)
        {
            var libelle = (dto.Libelle ?? "").Trim();
            if (string.IsNullOrWhiteSpace(libelle))
                return BadRequest("Libelle is required.");

            var exists = await _context.Statuts.CountAsync(x => x.Libelle.ToLower() == libelle.ToLower()) > 0;
            if (exists)
                return BadRequest("Statut Already exist");

            var entity = new Statut
            {
                Libelle = libelle
            };

            _context.Statuts.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new StatutResponseDto
            {
                Id = entity.Id,
                Libelle = entity.Libelle
            });
        }

        [HttpPut("statuts/{id:int}")]
        public async Task<IActionResult> Update(int id, StatutUpdateDto dto)
        {
            var statut = await _context.Statuts.FirstOrDefaultAsync(x => x.Id == id);
            if (statut == null)
                return NotFound();

            var newLibelle = (dto.Libelle ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newLibelle))
                return BadRequest("Libelle is required.");

            var exists = await _context.Statuts
                .CountAsync(x => x.Id != id && x.Libelle.ToLower() == newLibelle.ToLower()) > 0;

            if (exists)
                return BadRequest("Statut Already exist");

            statut.Libelle = newLibelle;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("statuts/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var statut = await _context.Statuts.FirstOrDefaultAsync(x => x.Id == id);
            if (statut == null)
                return NotFound();

            var usedByUsers = await _context.Users.CountAsync(u => u.StatutId == id) > 0;
            if (usedByUsers)
                return BadRequest("Cannot delete statut: it is used by users.");

            _context.Statuts.Remove(statut);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
