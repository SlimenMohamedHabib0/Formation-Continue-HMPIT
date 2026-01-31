
using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string MotsCles { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Etat { get; set; } = null!;

        public DateTime? DatePublication { get; set; }

        [MaxLength(255)]
        public string? NomFichierPdf { get; set; }

        public byte[]? ContenuPdf { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<CourseProfessor> CourseProfessors { get; set; } = new List<CourseProfessor>();
        public ICollection<QuestionCourse> CourseQuestions { get; set; } = new List<QuestionCourse>();
        public ICollection<TentativeQcm> TentativesQcm { get; set; } = new List<TentativeQcm>();

    }
}
