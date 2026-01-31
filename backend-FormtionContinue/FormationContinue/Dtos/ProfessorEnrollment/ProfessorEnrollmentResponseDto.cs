namespace FormationContinue.Dtos.ProfessorEnrollment
{
    public class ProfessorEnrollmentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public int UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string Statut { get; set; } = null!;
        public DateTime DateDemande { get; set; }
        public DateTime? DateDecision { get; set; }
    }
}
