using FormationContinue.Data;
using FormationContinue.Dtos.Admin;
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
    public class AdminProfessorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminProfessorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("professors")]
        public async Task<ActionResult<List<ProfessorResponseDto>>> GetAll([FromQuery] string? search)
        {
            var q = _context.Users
    .AsNoTracking()
    .Where(u => u.Role == "PROFESSOR");

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(u =>
                    u.FullName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

            var profs = await q
                .Select(u => new ProfessorResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(profs);

        }

        [HttpPost("professors")]
        public async Task<ActionResult<ProfessorResponseDto>> Create(ProfessorCreateDto dto)
        {
            var email = dto.Email.Trim();
            var fullName = dto.FullName.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (user.Role == "PROFESSOR")
                    return BadRequest("Professor already exists.");

                if (user.Role == "ADMIN")
                    return BadRequest("Cannot convert admin to professor.");

                if (user.Role != "USER")
                    return BadRequest("User role not supported for conversion.");

                user.FullName = fullName;
                user.Email = email;
                user.Role = "PROFESSOR";

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password);
                }

                await _context.SaveChangesAsync();

                return Ok(new ProfessorResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required.");

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Role = "PROFESSOR"
            };

            newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, dto.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { }, new ProfessorResponseDto
            {
                Id = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = newUser.Role
            });
        }


        [HttpPut("professors/{id}")]
        public async Task<IActionResult> Update(int id, ProfessorUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            if (user.Role != "PROFESSOR")
                return BadRequest("User is not a professor.");

            var newEmail = dto.Email.Trim();

            var emailUsedByOther = (await _context.Users.CountAsync(u =>
                u.Id != id && u.Email.ToLower() == newEmail.ToLower())) > 0;

            if (emailUsedByOther)
                return BadRequest("Email Already Used");

            user.FullName = dto.FullName.Trim();
            user.Email = newEmail;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password.Trim());
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("professors/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            if (user.Role != "PROFESSOR")
                return BadRequest("User is not a professor.");

            var profCourseIds = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == id)
                .Select(cp => cp.CourseId)
                .Distinct()
                .ToListAsync();

            if (profCourseIds.Count > 0)
            {
                var nonDraftCourseIds = await _context.Courses
                    .Where(c => profCourseIds.Contains(c.Id) && c.Etat != "DRAFT")
                    .Select(c => c.Id)
                    .ToListAsync();

                if (nonDraftCourseIds.Count > 0)
                {
                    foreach (var courseId in nonDraftCourseIds)
                    {
                        var otherProfCount = await _context.CourseProfessors
                            .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId != id);

                        if (otherProfCount == 0)
                            return BadRequest("Cannot delete professor: some published courses would have no professors.");
                    }
                }

                var links = await _context.CourseProfessors
                    .Where(cp => cp.ProfessorId == id)
                    .ToListAsync();

                _context.CourseProfessors.RemoveRange(links);
                await _context.SaveChangesAsync();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
