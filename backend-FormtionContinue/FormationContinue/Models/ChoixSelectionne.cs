using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class ChoixSelectionne
    {
        [Required]
        public int TentativeQcmId { get; set; }
        public TentativeQcm TentativeQcm { get; set; } = null!;

        [Required]
        public int ChoixId { get; set; }
        public Choix Choix { get; set; } = null!;
    }
}
