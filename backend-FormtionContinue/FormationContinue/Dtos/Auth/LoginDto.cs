using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(8), MaxLength(64)]
        public string Password { get; set; } = null!;

    }
}
