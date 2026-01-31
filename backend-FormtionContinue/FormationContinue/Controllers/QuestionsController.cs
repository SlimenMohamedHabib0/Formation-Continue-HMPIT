using FormationContinue.Data;
using FormationContinue.Dtos.Question;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FormationContinue.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "PROFESSOR")]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("courses/{courseId}/questions")]
        public async Task<ActionResult<List<QuestionResponseDto>>> GetCourseQuestions(int courseId)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var ownsCourseCount = await _context.CourseProfessors
    .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorIdInt);

            if (ownsCourseCount == 0)
                return Forbid();


            var questions = await _context.CourseQuestions
                .AsNoTracking()
                .Where(qc => qc.CourseId == courseId)
                .Include(qc => qc.Question)
                    .ThenInclude(q => q.Choix)
                .Select(qc => qc.Question)
                .ToListAsync();

            var result = questions.Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Enonce = q.Enonce,
                Points = q.Points,
                Choix = q.Choix.Select(c => new QuestionChoiceCreateDto
                {
                    Libelle = c.Libelle,
                    EstCorrect = c.EstCorrect
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        [HttpPost("courses/{id}/qcm/publish")]

        public async Task<IActionResult> Publish(int id)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null)
                return Unauthorized();
            if (!int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var course = await _context.CourseProfessors
                .Where(cp => cp.ProfessorId == professorIdInt && cp.CourseId == id)
                .Select(cp => cp.Course)
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            if (course.Etat != "DRAFT")
                return BadRequest("Only draft courses can be published.");

            if (course.ContenuPdf == null || course.ContenuPdf.Length == 0 || string.IsNullOrWhiteSpace(course.NomFichierPdf))
                return BadRequest("You must attach a PDF before publishing.");

            var questions = await _context.CourseQuestions
                .Where(qc => qc.CourseId == id)
                .Include(qc => qc.Question)
                    .ThenInclude(q => q.Choix)
                .Select(qc => qc.Question)
                .AsNoTracking()
                .ToListAsync();

            var totalPoints = questions.Sum(q => q.Points);

            if (totalPoints != 20)
                return BadRequest($"QCM invalid: total points = {totalPoints}, expected 20.");

            foreach (var q in questions)
            {
                var correctCount = q.Choix.Count(c => c.EstCorrect);

                if (correctCount == 0)
                    return BadRequest($"QCM invalid: question {q.Id} has no correct choices.");

                if (q.Choix.Count > 0 && correctCount == q.Choix.Count)
                    return BadRequest($"QCM invalid: question {q.Id} all choices are correct.");

                var hasDuplicateChoices = q.Choix
                    .GroupBy(c => (c.Libelle ?? "").Trim().ToLower())
                    .Any(g => g.Key != "" && g.Count() > 1);

                if (hasDuplicateChoices)
                    return BadRequest($"QCM invalid: question {q.Id} has duplicate choice text.");
            }

            course.Etat = "PUBLISHED";
            course.DatePublication = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("courses/{courseId}/questions")]
        public async Task<ActionResult<QuestionResponseDto>> CreateQuestionForCourse(
    int courseId,
    QuestionCreateDto dto)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var ownsCourseCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorIdInt);

            if (ownsCourseCount == 0)
                return Forbid();

            var attemptsCount = await _context.TentativesQcm.CountAsync(t => t.CourseId == courseId);
            if (attemptsCount > 0)
                return BadRequest("Cannot modify QCM: attempts exist.");

            var courseEtat = await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.Etat)
                .FirstOrDefaultAsync();

            if (courseEtat == null)
                return NotFound("Course not found.");

            if (courseEtat != "DRAFT")
                return BadRequest("Only draft courses can be modified.");

            if (dto == null)
                return BadRequest("Invalid payload.");

            if (string.IsNullOrWhiteSpace(dto.Enonce))
                return BadRequest("Enoncé is required.");

            if (dto.Points <= 0)
                return BadRequest("Points must be > 0.");

            if (dto.Choix == null || dto.Choix.Count < 2)
                return BadRequest("At least 2 choices are required.");

            var correctCount = dto.Choix.Count(c => c.EstCorrect);
            if (correctCount == 0)
                return BadRequest("At least 1 correct choice is required.");

            if (correctCount == dto.Choix.Count)
                return BadRequest("Not all choices can be correct.");

            var hasDuplicateChoices = dto.Choix
                .GroupBy(c => (c.Libelle ?? "").Trim().ToLower())
                .Any(g => g.Key != "" && g.Count() > 1);

            if (hasDuplicateChoices)
                return BadRequest("Duplicate choice text is not allowed.");

            var currentTotal = await _context.CourseQuestions
                .Where(qc => qc.CourseId == courseId)
                .Select(qc => qc.Question.Points)
                .SumAsync();

            if (currentTotal + dto.Points > 20)
                return BadRequest("Total points cannot exceed 20.");

            var question = new Question
            {
                Enonce = dto.Enonce.Trim(),
                Points = dto.Points
            };

            foreach (var c in dto.Choix)
            {
                if (string.IsNullOrWhiteSpace(c.Libelle))
                    return BadRequest("Choice text is required.");

                question.Choix.Add(new Choix
                {
                    Libelle = c.Libelle.Trim(),
                    EstCorrect = c.EstCorrect
                });
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            _context.CourseQuestions.Add(new QuestionCourse
            {
                CourseId = courseId,
                QuestionId = question.Id
            });

            await _context.SaveChangesAsync();

            return Ok(new QuestionResponseDto
            {
                Id = question.Id,
                Enonce = question.Enonce,
                Points = question.Points,
                Choix = question.Choix.Select(x => new QuestionChoiceCreateDto
                {
                    Libelle = x.Libelle,
                    EstCorrect = x.EstCorrect
                }).ToList()
            });
        }


        [HttpPut("courses/{courseId}/questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestionForCourse(
    int courseId,
    int questionId,
    QuestionUpdateDto dto)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var ownsCourseCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorIdInt);

            if (ownsCourseCount == 0)
                return Forbid();

            var attemptsCount = await _context.TentativesQcm.CountAsync(t => t.CourseId == courseId);
            if (attemptsCount > 0)
                return BadRequest("Cannot modify QCM: attempts exist.");

            var link = await _context.CourseQuestions
                .FirstOrDefaultAsync(qc => qc.CourseId == courseId && qc.QuestionId == questionId);

            if (link == null)
                return NotFound();
            if (dto.Points <= 0)
                return BadRequest("Points must be > 0.");

            var currentTotal = await _context.CourseQuestions
                .Where(qc => qc.CourseId == courseId)
                .Select(qc => qc.Question)
                .SumAsync(q => q.Id == questionId ? 0 : q.Points);

            if (currentTotal + dto.Points > 20)
                return BadRequest("Total points cannot exceed 20.");


            var usageCount = await _context.CourseQuestions
                .CountAsync(qc => qc.QuestionId == questionId);

            if (usageCount > 1)
            {
                var newQuestion = new Question
                {
                    Enonce = dto.Enonce.Trim(),
                    Points = dto.Points
                };

                foreach (var c in dto.Choix)
                {
                    newQuestion.Choix.Add(new Choix
                    {
                        Libelle = c.Libelle.Trim(),
                        EstCorrect = c.EstCorrect
                    });
                }

                _context.Questions.Add(newQuestion);
                await _context.SaveChangesAsync();

                link.QuestionId = newQuestion.Id;
            }
            else
            {
                var question = await _context.Questions
                    .Include(q => q.Choix)
                    .FirstAsync(q => q.Id == questionId);

                question.Enonce = dto.Enonce.Trim();
                question.Points = dto.Points;

                _context.Choix.RemoveRange(question.Choix);

                foreach (var c in dto.Choix)
                {
                    question.Choix.Add(new Choix
                    {
                        Libelle = c.Libelle.Trim(),
                        EstCorrect = c.EstCorrect
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("courses/{courseId}/questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestionFromCourse(
            int courseId,
            int questionId)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var ownsCourseCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorIdInt);

            if (ownsCourseCount == 0)
                return Forbid();

            var attemptsCount = await _context.TentativesQcm.CountAsync(t => t.CourseId == courseId);
            if (attemptsCount > 0)
                return BadRequest("Cannot modify QCM: attempts exist.");

            var link = await _context.CourseQuestions
                .FirstOrDefaultAsync(qc => qc.CourseId == courseId && qc.QuestionId == questionId);

            if (link == null)
                return NotFound();

            _context.CourseQuestions.Remove(link);

            var stillUsed = await _context.CourseQuestions
                .CountAsync(qc => qc.QuestionId == questionId && qc.CourseId != courseId);

            if (stillUsed == 0)
            {
                var question = await _context.Questions
                    .Include(q => q.Choix)
                    .FirstAsync(q => q.Id == questionId);

                _context.Choix.RemoveRange(question.Choix);
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("courses/{courseId}/qcm-validity")]
        public async Task<IActionResult> GetQcmValidity(int courseId)
        {
            var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (professorId == null || !int.TryParse(professorId, out var professorIdInt))
                return Unauthorized();

            var ownsCourseCount = await _context.CourseProfessors
                .CountAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorIdInt);

            if (ownsCourseCount == 0)
                return Forbid();

            var questions = await _context.CourseQuestions
                .Where(qc => qc.CourseId == courseId)
                .Include(qc => qc.Question)
                    .ThenInclude(q => q.Choix)
                .Select(qc => qc.Question)
                .AsNoTracking()
                .ToListAsync();

            var totalPoints = questions.Sum(q => q.Points);

            var warnings = new List<string>();

            foreach (var q in questions)
            {
                var correctCount = q.Choix.Count(c => c.EstCorrect);

                if (correctCount == 0)
                    warnings.Add($"Question {q.Id}: no correct choices.");

                if (correctCount == q.Choix.Count)
                    warnings.Add($"Question {q.Id}: all choices are correct.");

                var hasDuplicateChoices = q.Choix
                    .GroupBy(c => c.Libelle.Trim().ToLower())
                    .Any(g => g.Count() > 1);

                if (hasDuplicateChoices)
                    warnings.Add($"Question {q.Id}: duplicate choice text.");
            }

            return Ok(new
            {
                TotalPoints = totalPoints,
                IsValid = totalPoints == 20,
                Warnings = warnings
            });
        }


    }
}
