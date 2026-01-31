using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Admin
{
    public class ProfessorCreateDto
    {
        [Required, MinLength(3), MaxLength(64)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(8), MaxLength(64)]
        public string Password { get; set; } = null!;
    }
}
