namespace FormationContinue.Dtos.Admin
{
    public class ProfessorResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public int ServiceId { get; set; }
        public string ServiceLibelle { get; set; } = null!;

        public int StatutId { get; set; }
        public string StatutLibelle { get; set; } = null!;
    }
}
