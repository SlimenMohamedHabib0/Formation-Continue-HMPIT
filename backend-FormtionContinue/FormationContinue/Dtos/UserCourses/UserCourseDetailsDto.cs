namespace FormationContinue.Dtos.UserCourses
{
    public class UserCourseDetailsDto
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

        public List<UserCourseProfessorDto> Professors { get; set; } = new();
    }

    public class UserCourseProfessorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
