namespace FormationContinue.Dtos.UserCourses
{
    public class UserCourseListItemDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MotsCles { get; set; } = null!;
        public DateTime? DatePublication { get; set; }

        public int CategoryId { get; set; }
        public string CategoryLibelle { get; set; } = null!;

        public string? NomFichierPdf { get; set; }

        public string? VideoFileName { get; set; }
        public string? VideoPath { get; set; }
        public string? VideoMimeType { get; set; }
    }
}
