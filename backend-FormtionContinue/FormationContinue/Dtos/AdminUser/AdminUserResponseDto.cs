namespace FormationContinue.Dtos.AdminUser
{
    public class AdminUserResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int ServiceId { get; set; }
        public string ServiceLibelle { get; set; } = null!;
        public int StatutId { get; set; }
        public string StatutLibelle { get; set; } = null!;
    }
}
