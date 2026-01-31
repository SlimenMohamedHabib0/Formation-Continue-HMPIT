using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Choix
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Libelle { get; set; } = null!;

        public bool EstCorrect { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public ICollection<ChoixSelectionne> ChoixSelectionnes { get; set; } = new List<ChoixSelectionne>();
    }
}
