using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Service
{
    public class ServiceUpdateDto
    {
        [Required]
        public string Libelle { get; set; } = null!;
    }
}
