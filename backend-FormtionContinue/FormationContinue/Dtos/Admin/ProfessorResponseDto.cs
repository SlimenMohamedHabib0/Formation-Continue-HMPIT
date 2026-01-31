using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Admin
{
    public class ProfessorResponseDto
    {
        
        public int Id { get; set; }
        
        public string FullName { get; set; } = null!;
        
        public string Email { get; set; } = null!;
        
        public string Role { get; set; } = null!;
    }
}
