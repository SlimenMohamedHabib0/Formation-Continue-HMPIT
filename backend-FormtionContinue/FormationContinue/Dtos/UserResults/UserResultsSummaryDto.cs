namespace FormationContinue.Dtos.UserResults
{
    public class UserResultsSummaryDto
    {
        public int NbCoursesEnrolled { get; set; }
        public int NbCoursesPending { get; set; }
        public int NbCoursesAccepted { get; set; }
        public int NbCoursesRefused { get; set; }
        public int NbCoursesCompleted { get; set; }

        public int NbPassedTests { get; set; }
        public int NbFailedTests { get; set; }
        public double? AverageNote { get; set; }
        public double? LastTestNote { get; set; }
        public string? LastTestStatus { get; set; }
    }
}
