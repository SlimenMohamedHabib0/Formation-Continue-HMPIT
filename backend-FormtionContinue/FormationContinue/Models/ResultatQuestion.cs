using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class ResultatQuestion
    {
        public int Id { get; set; }

        [Required]
        public bool EstCorrect { get; set; }

        [Required]
        public double PointsObtenus { get; set; }

        [Required]
        public int TentativeQcmId { get; set; }
        public TentativeQcm TentativeQcm { get; set; } = null!;

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
