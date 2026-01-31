using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Category
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Libelle { get; set; } = null!;
    }
}
