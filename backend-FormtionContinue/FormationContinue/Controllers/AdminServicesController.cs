using FormationContinue.Data;
using FormationContinue.Dtos.Service;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminServicesController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("services")]
        public async Task<ActionResult<List<ServiceResponseDto>>> GetAll([FromQuery] string? search)
        {
            var q = _context.Services.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x => x.Libelle.ToLower().Contains(s));
            }

            var list = await q
                .OrderBy(x => x.Libelle)
                .Select(x => new ServiceResponseDto
                {
                    Id = x.Id,
                    Libelle = x.Libelle
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("services")]
        public async Task<ActionResult<ServiceResponseDto>> Create(ServiceCreateDto dto)
        {
            var libelle = (dto.Libelle ?? "").Trim();
            if (string.IsNullOrWhiteSpace(libelle))
                return BadRequest("Libelle is required.");

            var exists = await _context.Services.CountAsync(x => x.Libelle.ToLower() == libelle.ToLower()) > 0;
            if (exists)
                return BadRequest("Service Already exist");

            var entity = new Service
            {
                Libelle = libelle
            };

            _context.Services.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new ServiceResponseDto
            {
                Id = entity.Id,
                Libelle = entity.Libelle
            });
        }

        [HttpPut("services/{id:int}")]
        public async Task<IActionResult> Update(int id, ServiceUpdateDto dto)
        {
            var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
                return NotFound();

            var newLibelle = (dto.Libelle ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newLibelle))
                return BadRequest("Libelle is required.");

            var exists = await _context.Services
                .CountAsync(x => x.Id != id && x.Libelle.ToLower() == newLibelle.ToLower()) > 0;

            if (exists)
                return BadRequest("Service Already exist");

            service.Libelle = newLibelle;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("services/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
                return NotFound();

            var usedByUsers = await _context.Users.CountAsync(u => u.ServiceId == id) > 0;
            if (usedByUsers)
                return BadRequest("Cannot delete service: it is used by users.");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
