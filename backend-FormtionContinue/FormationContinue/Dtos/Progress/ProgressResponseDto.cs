namespace FormationContinue.Dtos.Progress
{
    public class ProgressResponseDto
    {
        public int CourseId { get; set; }
        public int EnrollmentId { get; set; }
        public int DernierePageAtteinte { get; set; }
        public DateTime? DateCompletion { get; set; }
    }
}
