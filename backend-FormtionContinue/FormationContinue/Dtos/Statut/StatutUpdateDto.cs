using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Statut
{
    public class StatutUpdateDto
    {
        [Required]
        public string Libelle { get; set; } = null!;
    }
}
