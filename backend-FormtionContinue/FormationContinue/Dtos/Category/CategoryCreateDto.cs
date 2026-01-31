using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Category
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Libelle { get; set; } = null!;
    }
}
