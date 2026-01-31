namespace FormationContinue.Dtos.Enrollment
{
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public string Statut { get; set; } = null!;
        public DateTime DateDemande { get; set; }
        public DateTime? DateDecision { get; set; }
    }
}
