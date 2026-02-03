namespace FormationContinue.Dtos.AdminStats
{
    public class AdminDashboardDto
    {
        public int NbUsers { get; set; }
        public int NbProfessors { get; set; }
        public int NbAdmins { get; set; }

        public int NbCategories { get; set; }
        public int NbCoursesTotal { get; set; }
        public int NbCoursesDraft { get; set; }
        public int NbCoursesPublished { get; set; }

        public int NbEnrollmentsTotal { get; set; }
        public int NbEnrollmentsPending { get; set; }
        public int NbEnrollmentsAccepted { get; set; }
        public int NbEnrollmentsRefused { get; set; }

        public int NbAttemptsTotal { get; set; }
        public int NbAttemptsPassed { get; set; }
        public int NbAttemptsFailed { get; set; }
        public double? AverageNote { get; set; }
        public double SuccessRatePercent { get; set; }

        public List<CountItemDto> TopCoursesByEnrollments { get; set; } = new();
        public List<CountItemDto> TopCategoriesByCourses { get; set; } = new();
        public List<CountItemDto> TopCategoriesByEnrollments { get; set; } = new();
        public List<CountItemDto> TopServicesByEnrollments { get; set; } = new();
        public List<CountItemDto> TopStatutsByEnrollments { get; set; } = new();
    }

    public class CountItemDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = null!;
        public int Count { get; set; }
    }
}
