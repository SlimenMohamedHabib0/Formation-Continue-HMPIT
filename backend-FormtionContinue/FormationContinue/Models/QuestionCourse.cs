using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class QuestionCourse
    {
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
