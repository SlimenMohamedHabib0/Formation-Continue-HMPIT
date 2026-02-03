using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Statut
{
    public class StatutCreateDto
    {
        [Required]
        public string Libelle { get; set; } = null!;
    }
}
