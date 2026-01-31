using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Enonce { get; set; } = null!;

        [Required]
        public double Points { get; set; }

        public ICollection<Choix> Choix { get; set; } = new List<Choix>();
        public ICollection<QuestionCourse> QuestionCourses { get; set; } = new List<QuestionCourse>();
        public ICollection<ResultatQuestion> ResultatQuestions { get; set; } = new List<ResultatQuestion>();
    }
}
