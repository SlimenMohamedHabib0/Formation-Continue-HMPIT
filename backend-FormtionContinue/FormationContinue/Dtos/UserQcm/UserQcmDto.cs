namespace FormationContinue.Dtos.UserQcm
{
    public class UserQcmDto
    {
        public int CourseId { get; set; }
        public double TotalPoints { get; set; }
        public List<UserQcmQuestionDto> Questions { get; set; } = new();
    }

    public class UserQcmQuestionDto
    {
        public int QuestionId { get; set; }
        public string Enonce { get; set; } = null!;
        public double Points { get; set; }
        public List<UserQcmChoiceDto> Choix { get; set; } = new();
    }

    public class UserQcmChoiceDto
    {
        public int ChoixId { get; set; }
        public string Libelle { get; set; } = null!;
    }
}
