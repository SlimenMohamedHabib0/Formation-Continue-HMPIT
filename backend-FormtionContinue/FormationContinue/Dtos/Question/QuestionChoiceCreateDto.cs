namespace FormationContinue.Dtos.Question
{
    public class QuestionChoiceCreateDto
    {
        public string Libelle { get; set; } = null!;
        public bool EstCorrect { get; set; }
    }
}
