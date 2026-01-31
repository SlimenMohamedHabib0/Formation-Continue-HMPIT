namespace FormationContinue.Dtos.UserResults
{
    public class UserAttemptDto
    {
        public int TentativeId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitre { get; set; } = null!;
        public DateTime DateTentative { get; set; }
        public double NoteSur20 { get; set; }
        public string StatutTentative { get; set; } = null!;
    }
}
