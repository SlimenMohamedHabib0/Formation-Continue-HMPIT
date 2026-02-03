using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Libelle { get; set; } = null!;
    }
}
