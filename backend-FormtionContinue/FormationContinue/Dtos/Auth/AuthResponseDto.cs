using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Auth
{
    public class AuthResponseDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string ServiceLibelle { get; set; } = null!;

        [Required]
        public int StatutId { get; set; }

        [Required]
        public string StatutLibelle { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;
    }
}
