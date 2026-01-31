using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Statut { get; set; } = "PENDING";

        [Required]
        public DateTime DateDemande { get; set; } = DateTime.UtcNow;

        public DateTime? DateDecision { get; set; }

        public int? DecisionProfessorId { get; set; }
        public User? DecisionProfessor { get; set; }
    }
}
