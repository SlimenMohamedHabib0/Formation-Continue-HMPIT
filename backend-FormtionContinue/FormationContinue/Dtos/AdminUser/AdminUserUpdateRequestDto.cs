using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.AdminUser
{
    public class AdminUserUpdateRequestDto
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? Password { get; set; }
    }
}
