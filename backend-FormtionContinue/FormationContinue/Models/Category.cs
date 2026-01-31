using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Libelle { get; set; } = null!;
    }
}
