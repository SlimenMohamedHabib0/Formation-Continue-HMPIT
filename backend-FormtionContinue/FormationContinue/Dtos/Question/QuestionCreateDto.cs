using System.Collections.Generic;

namespace FormationContinue.Dtos.Question
{
    public class QuestionCreateDto
    {
        public string Enonce { get; set; } = null!;
        public double Points { get; set; }
        public List<QuestionChoiceCreateDto> Choix { get; set; } = new();
    }
}
