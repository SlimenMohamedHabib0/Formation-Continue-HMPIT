using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class TentativeQcm
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateTentative { get; set; }

        public double NoteSur20 { get; set; }

        [Required]
        [MaxLength(50)]
        public string StatutTentative { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<ChoixSelectionne> ChoixSelectionnes { get; set; } = new List<ChoixSelectionne>();
        public ICollection<ResultatQuestion> ResultatQuestions { get; set; } = new List<ResultatQuestion>();
    }
}
