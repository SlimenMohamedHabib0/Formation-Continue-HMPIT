using System;
using System.ComponentModel.DataAnnotations;
namespace FormationContinue.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(64)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Role { get; set; } = null!;

        [Required, MaxLength(400)]
        public string PasswordHash { get; set; } = null!;

    
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
