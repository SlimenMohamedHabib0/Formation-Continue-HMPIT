namespace FormationContinue.Dtos.ProfessorStats
{
    public class ProfessorDashboardDto
    {
        public int NbMyCoursesTotal { get; set; }
        public int NbMyCoursesDraft { get; set; }
        public int NbMyCoursesPublished { get; set; }

        public int NbEnrollmentsTotal { get; set; }
        public int NbEnrollmentsPending { get; set; }
        public int NbEnrollmentsAccepted { get; set; }
        public int NbEnrollmentsRefused { get; set; }

        public int NbAttemptsTotal { get; set; }
        public int NbAttemptsPassed { get; set; }
        public int NbAttemptsFailed { get; set; }
        public double? AverageNote { get; set; }
        public double SuccessRatePercent { get; set; }

        public List<CourseCountItemDto> TopCoursesByEnrollments { get; set; } = new();
        public List<CourseSuccessItemDto> SuccessRateByCourse { get; set; } = new();
    }

    public class CourseCountItemDto
    {
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public int Count { get; set; }
    }

    public class CourseSuccessItemDto
    {
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public int Attempts { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public double SuccessRatePercent { get; set; }
    }
}
