namespace FormationContinue.Dtos.AdminEnrollment
{
    public class AdminEnrollmentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public int CategoryId { get; set; }

        public int UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;

        public string Statut { get; set; } = null!;
        public DateTime DateDemande { get; set; }
        public DateTime? DateDecision { get; set; }

        public int? DecisionProfessorId { get; set; }
        public string? DecisionProfessorFullName { get; set; }
        public string? DecisionProfessorEmail { get; set; }
    }
}
