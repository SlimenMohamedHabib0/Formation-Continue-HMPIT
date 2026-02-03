using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Service
{
    public class ServiceCreateDto
    {
        [Required]
        public string Libelle { get; set; } = null!;
    }
}
