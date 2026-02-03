using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Admin
{
    public class ProfessorUpdateDto
    {
        [Required, MinLength(3), MaxLength(64)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int StatutId { get; set; }
    }
}
