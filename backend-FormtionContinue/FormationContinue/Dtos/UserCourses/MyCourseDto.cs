namespace FormationContinue.Dtos.UserCourses
{
    public class MyCourseDto
    {
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public string CourseEtat { get; set; } = null!;

        public int CategoryId { get; set; }
        public string CategoryLibelle { get; set; } = null!;

        public int EnrollmentId { get; set; }
        public string EnrollmentStatut { get; set; } = null!;
        public DateTime DateDemande { get; set; }
        public DateTime? DateDecision { get; set; }

        public int DernierePageAtteinte { get; set; }
        public DateTime? DateCompletion { get; set; }
    }
}
