using FormationContinue.Data;
using FormationContinue.Dtos.AdminUser;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminUsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<AdminUserResponseDto>>> GetAll([FromQuery] string? search, [FromQuery] string? role)
        {
            var q = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(role))
            {
                var r = role.Trim().ToUpper();
                q = q.Where(u => u.Role.ToUpper() == r);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(u =>
                    u.FullName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

            var users = await q
                .OrderByDescending(u => u.Id)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    ServiceId = u.ServiceId,
                    ServiceLibelle = u.Service.Libelle,
                    StatutId = u.StatutId,
                    StatutLibelle = u.Statut.Libelle
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    ServiceId = u.ServiceId,
                    ServiceLibelle = u.Service.Libelle,
                    StatutId = u.StatutId,
                    StatutLibelle = u.Statut.Libelle
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> Update(int id, AdminUserUpdateRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            var newRole = dto.Role.Trim().ToUpper();
            if (newRole != "ADMIN" && newRole != "PROFESSOR" && newRole != "USER")
                return BadRequest("Role invalide.");

            var email = dto.Email.Trim().ToLower();
            var emailExists = await _context.Users.CountAsync(u => u.Id != id && u.Email.ToLower() == email) > 0;
            if (emailExists)
                return BadRequest("Email déjà utilisé.");

            var isAdminNow = user.Role.ToUpper() == "ADMIN";
            if (isAdminNow && newRole != "ADMIN")
            {
                var adminsCount = await _context.Users.CountAsync(u => u.Role.ToUpper() == "ADMIN");
                if (adminsCount <= 1)
                    return BadRequest("Impossible de retirer le dernier admin.");
            }

            var serviceExists = await _context.Services.CountAsync(s => s.Id == dto.ServiceId) > 0;
            if (!serviceExists)
                return BadRequest("Service introuvable.");

            var statutExists = await _context.Statuts.CountAsync(s => s.Id == dto.StatutId) > 0;
            if (!statutExists)
                return BadRequest("Statut introuvable.");

            user.FullName = dto.FullName.Trim();
            user.Email = dto.Email.Trim();
            user.Role = newRole;
            user.ServiceId = dto.ServiceId;
            user.StatutId = dto.StatutId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = new PasswordHasher<User>()
                    .HashPassword(user, dto.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new AdminUserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                ServiceId = user.ServiceId,
                ServiceLibelle = user.Service.Libelle,
                StatutId = user.StatutId,
                StatutLibelle = user.Statut.Libelle
            });
        }
    }
}
