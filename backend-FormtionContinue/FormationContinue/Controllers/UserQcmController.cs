using FormationContinue.Data;
using FormationContinue.Dtos.UserQcm;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api/user-courses")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserQcmController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserQcmController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{courseId:int}/qcm")]
        public async Task<ActionResult<UserQcmDto>> GetQcm(int courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var courseCount = await _context.Courses
                .AsNoTracking()
                .CountAsync(c => c.Id == courseId && c.Etat == "PUBLISHED");

            if (courseCount == 0)
                return NotFound("Course not available.");

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.CourseId == courseId &&
                    e.UserId == userId &&
                    e.Statut == "ACCEPTEE");

            if (enrollment == null)
                return Forbid();

            var progress = await _context.Progress
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id);

            if (progress == null)
                return BadRequest("Progress not found.");

            if (progress.DateCompletion == null)
                return BadRequest("Course not completed.");

            var lastAttempt = await _context.TentativesQcm
                .AsNoTracking()
                .Where(t => t.CourseId == courseId && t.UserId == userId)
                .OrderByDescending(t => t.DateTentative)
                .FirstOrDefaultAsync();

            if (lastAttempt != null)
            {
                if (lastAttempt.StatutTentative == "REUSSI")
                    return BadRequest("Test already passed.");

                if (lastAttempt.StatutTentative == "ECHOUE")
                {
                    if (progress.DateCompletion <= lastAttempt.DateTentative)
                        return BadRequest("You must re-read and complete the course before retrying the test.");
                }
            }

            var questionIds = await _context.CourseQuestions
                .AsNoTracking()
                .Where(qc => qc.CourseId == courseId)
                .Select(qc => qc.QuestionId)
                .ToListAsync();

            if (questionIds.Count == 0)
                return BadRequest("QCM not found.");

            var questions = await _context.Questions
                .AsNoTracking()
                .Where(q => questionIds.Contains(q.Id))
                .Include(q => q.Choix)
                .ToListAsync();

            var totalPoints = questions.Sum(q => q.Points);
            if (totalPoints != 20)
                return BadRequest($"QCM invalid: total points = {totalPoints}, expected 20.");

            var dto = new UserQcmDto
            {
                CourseId = courseId,
                TotalPoints = totalPoints,
                Questions = questions.Select(q => new UserQcmQuestionDto
                {
                    QuestionId = q.Id,
                    Enonce = q.Enonce,
                    Points = q.Points,
                    Choix = q.Choix.Select(c => new UserQcmChoiceDto
                    {
                        ChoixId = c.Id,
                        Libelle = c.Libelle
                    }).ToList()
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost("{courseId:int}/qcm/submit")]
        public async Task<ActionResult<UserQcmSubmitResultDto>> Submit(int courseId, UserQcmSubmitDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            // ✅ ORACLE FIX: no AnyAsync
            var courseCount = await _context.Courses
                .AsNoTracking()
                .CountAsync(c => c.Id == courseId && c.Etat == "PUBLISHED");

            if (courseCount == 0)
                return NotFound("Course not available.");

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.CourseId == courseId &&
                    e.UserId == userId &&
                    e.Statut == "ACCEPTEE");

            if (enrollment == null)
                return Forbid();

            var progress = await _context.Progress
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id);

            if (progress == null)
                return BadRequest("Progress not found.");

            if (progress.DateCompletion == null)
                return BadRequest("Course not completed.");

            var lastAttempt = await _context.TentativesQcm
                .AsNoTracking()
                .Where(t => t.CourseId == courseId && t.UserId == userId)
                .OrderByDescending(t => t.DateTentative)
                .FirstOrDefaultAsync();

            if (lastAttempt != null)
            {
                if (lastAttempt.StatutTentative == "REUSSI")
                    return BadRequest("Test already passed.");

                if (lastAttempt.StatutTentative == "ECHOUE")
                {
                    if (progress.DateCompletion <= lastAttempt.DateTentative)
                        return BadRequest("You must re-read and complete the course before retrying the test.");
                }
            }

            var questionIds = await _context.CourseQuestions
                .AsNoTracking()
                .Where(qc => qc.CourseId == courseId)
                .Select(qc => qc.QuestionId)
                .ToListAsync();

            if (questionIds.Count == 0)
                return BadRequest("QCM not found.");

            var questions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id))
                .Include(q => q.Choix)
                .ToListAsync();

            var totalPoints = questions.Sum(q => q.Points);
            if (totalPoints != 20)
                return BadRequest($"QCM invalid: total points = {totalPoints}, expected 20.");

            var answers = dto.Answers ?? new();
            if (answers.Count == 0)
                return BadRequest("Answers are required.");

            var attempt = new TentativeQcm
            {
                CourseId = courseId,
                UserId = userId,
                DateTentative = DateTime.UtcNow,
                NoteSur20 = 0,
                StatutTentative = "ECHOUE"
            };

            _context.TentativesQcm.Add(attempt);
            await _context.SaveChangesAsync();

            double note = 0;

            foreach (var q in questions)
            {
                var answer = answers.FirstOrDefault(a => a.QuestionId == q.Id);

                var selectedIds = (answer?.SelectedChoixIds ?? new List<int>())
                    .Distinct()
                    .ToList();

                var belongsCount = await _context.Choix
                    .CountAsync(c => selectedIds.Contains(c.Id) && c.QuestionId == q.Id);

                if (selectedIds.Count > 0 && belongsCount != selectedIds.Count)
                    return BadRequest($"Invalid choice selection for question {q.Id}.");

                foreach (var cid in selectedIds)
                {
                    _context.ChoixSelectionnes.Add(new ChoixSelectionne
                    {
                        TentativeQcmId = attempt.Id,
                        ChoixId = cid
                    });
                }

                var correctIds = q.Choix.Where(c => c.EstCorrect).Select(c => c.Id).OrderBy(x => x).ToList();
                var chosenIds = selectedIds.OrderBy(x => x).ToList();

                var isCorrect = correctIds.SequenceEqual(chosenIds);

                var pointsObt = isCorrect ? q.Points : 0;
                note += pointsObt;

                _context.ResultatQuestions.Add(new ResultatQuestion
                {
                    TentativeQcmId = attempt.Id,
                    QuestionId = q.Id,
                    EstCorrect = isCorrect,
                    PointsObtenus = pointsObt
                });
            }

            attempt.NoteSur20 = note;
            attempt.StatutTentative = (note >= 10) ? "REUSSI" : "ECHOUE";

            await _context.SaveChangesAsync();

            return Ok(new UserQcmSubmitResultDto
            {
                TentativeId = attempt.Id,
                NoteSur20 = attempt.NoteSur20,
                StatutTentative = attempt.StatutTentative
            });
        }
    }
}
