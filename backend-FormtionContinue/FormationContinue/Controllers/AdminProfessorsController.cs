using FormationContinue.Data;
using FormationContinue.Dtos.Admin;
using FormationContinue.Dtos.ProfessorStats;
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
        [HttpGet("professors/{id:int}/dashboard")]
        public async Task<ActionResult<ProfessorDashboardDto>> GetProfessorDashboard(int id)
        {
            var profCount = await _context.Users
    .AsNoTracking()
    .CountAsync(u => u.Id == id && u.Role == "PROFESSOR");

            if (profCount == 0)
                return NotFound("Professeur introuvable.");


           
            var myCourseIds = await _context.CourseProfessors
                .AsNoTracking()
                .Where(cp => cp.ProfessorId == id)
                .Select(cp => cp.CourseId)
                .Distinct()
                .ToListAsync();

            if (myCourseIds.Count == 0)
            {
                return Ok(new ProfessorDashboardDto
                {
                    NbMyCoursesTotal = 0,
                    NbMyCoursesDraft = 0,
                    NbMyCoursesPublished = 0,

                    NbEnrollmentsTotal = 0,
                    NbEnrollmentsPending = 0,
                    NbEnrollmentsAccepted = 0,
                    NbEnrollmentsRefused = 0,

                    NbAttemptsTotal = 0,
                    NbAttemptsPassed = 0,
                    NbAttemptsFailed = 0,
                    AverageNote = null,
                    SuccessRatePercent = 0,

                    TopCoursesByEnrollments = new List<CourseCountItemDto>(),
                    SuccessRateByCourse = new List<CourseSuccessItemDto>()
                });
            }

            var nbDraft = await _context.Courses.CountAsync(c => myCourseIds.Contains(c.Id) && c.Etat == "DRAFT");
            var nbPublished = await _context.Courses.CountAsync(c => myCourseIds.Contains(c.Id) && c.Etat == "PUBLISHED");

            var nbEnrollTotal = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId));
            var nbEnrollPending = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "PENDING");
            var nbEnrollAccepted = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "ACCEPTEE");
            var nbEnrollRefused = await _context.Enrollments.CountAsync(e => myCourseIds.Contains(e.CourseId) && e.Statut == "REFUSEE");

            var nbAttempts = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId));
            var nbPassed = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId) && t.StatutTentative == "REUSSI");
            var nbFailed = await _context.TentativesQcm.CountAsync(t => myCourseIds.Contains(t.CourseId) && t.StatutTentative == "ECHOUE");

            var avgNote = await _context.TentativesQcm
                .Where(t => myCourseIds.Contains(t.CourseId))
                .Select(t => (double?)t.NoteSur20)
                .AverageAsync();

            var successRate = nbAttempts == 0 ? 0 : (double)nbPassed * 100.0 / nbAttempts;
            var professorName = await _context.Users
    .Where(u => u.Id == id)
    .Select(u => u.FullName)
    .FirstAsync();
            var topCoursesByEnrollments = await _context.Enrollments
    .AsNoTracking()
    .Where(e => myCourseIds.Contains(e.CourseId))
    .GroupBy(e => new { e.CourseId, e.Course.Titre })
    .Select(g => new CourseCountItemDto
    {
        CourseId = g.Key.CourseId,
        CourseTitre = g.Key.Titre,
        Count = g.Count()
    })
    .OrderByDescending(x => x.Count)
    .Take(10)
    .ToListAsync();

            var successByCourseRaw = await _context.TentativesQcm
                .AsNoTracking()
                .Where(t => myCourseIds.Contains(t.CourseId))
                .GroupBy(t => new { t.CourseId, t.Course.Titre })
                .Select(g => new
                {
                    g.Key.CourseId,
                    g.Key.Titre,
                    Attempts = g.Count(),
                    Passed = g.Count(x => x.StatutTentative == "REUSSI"),
                    Failed = g.Count(x => x.StatutTentative == "ECHOUE")
                })
                .ToListAsync();

            var successRateByCourse = successByCourseRaw
                .Select(x => new CourseSuccessItemDto
                {
                    CourseId = x.CourseId,
                    CourseTitre = x.Titre,
                    Attempts = x.Attempts,
                    Passed = x.Passed,
                    Failed = x.Failed,
                    SuccessRatePercent = x.Attempts == 0 ? 0 : (double)x.Passed * 100.0 / x.Attempts
                })
                .OrderByDescending(x => x.Attempts)
                .Take(10)
                .ToList();


            return Ok(new ProfessorDashboardDto
            {
                NbMyCoursesTotal = myCourseIds.Count,
                NbMyCoursesDraft = nbDraft,
                NbMyCoursesPublished = nbPublished,

                NbEnrollmentsTotal = nbEnrollTotal,
                NbEnrollmentsPending = nbEnrollPending,
                NbEnrollmentsAccepted = nbEnrollAccepted,
                NbEnrollmentsRefused = nbEnrollRefused,

                NbAttemptsTotal = nbAttempts,
                NbAttemptsPassed = nbPassed,
                NbAttemptsFailed = nbFailed,
                AverageNote = avgNote,
                SuccessRatePercent = successRate,

                TopCoursesByEnrollments = topCoursesByEnrollments,
                SuccessRateByCourse = successRateByCourse
            });

        }

    }
}
