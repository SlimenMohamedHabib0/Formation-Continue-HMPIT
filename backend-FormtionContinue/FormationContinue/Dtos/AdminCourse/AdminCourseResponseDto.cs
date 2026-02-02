namespace FormationContinue.Dtos.AdminCourse
{
    public class AdminCourseResponseDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Etat { get; set; } = null!;
        public DateTime? DatePublication { get; set; }
        public int CategoryId { get; set; }
        public string? NomFichierPdf { get; set; }

        public string? VideoFileName { get; set; }
        public string? VideoMimeType { get; set; }

        public List<AdminCourseProfessorDto> Professors { get; set; } = new();
    }

    public class AdminCourseProfessorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
