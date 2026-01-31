namespace FormationContinue.Dtos.AdminAttempt
{
    public class AdminAttemptResponseDto
    {
        public int TentativeId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;

        public int UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;

        public DateTime DateTentative { get; set; }
        public double NoteSur20 { get; set; }
        public string StatutTentative { get; set; } = null!;
    }
}
