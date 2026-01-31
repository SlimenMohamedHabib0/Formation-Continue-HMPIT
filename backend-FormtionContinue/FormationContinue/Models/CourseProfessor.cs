using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class CourseProfessor
    {
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        public int ProfessorId { get; set; }
        public User Professor { get; set; } = null!;
    }
}
