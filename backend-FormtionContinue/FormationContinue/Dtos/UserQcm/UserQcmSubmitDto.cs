namespace FormationContinue.Dtos.UserQcm
{
    public class UserQcmSubmitDto
    {
        public List<UserQcmAnswerDto> Answers { get; set; } = new();
    }

    public class UserQcmAnswerDto
    {
        public int QuestionId { get; set; }
        public List<int> SelectedChoixIds { get; set; } = new();
    }
}
