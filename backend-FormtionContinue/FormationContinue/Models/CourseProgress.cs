using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class CourseProgress
    {
        public int Id { get; set; }

        [Required]
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        [Required]
        public int DernierePageAtteinte { get; set; } = 0;

        public DateTime? DateCompletion { get; set; }
    }
}
