using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Course
{
    public class CourseResponseDto
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string Titre { get; set; } = null!;

        public string Description { get; set; } = null!;

        [MaxLength(255)]
        public string MotsCles { get; set; } = null!;

        [MaxLength(50)]
        public string Etat { get; set; } = null!;

        public DateTime? DatePublication { get; set; }

        [MaxLength(255)]
        public string? NomFichierPdf { get; set; }

        [MaxLength(255)]
        public string? VideoFileName { get; set; }

        [MaxLength(600)]
        public string? VideoPath { get; set; }

        [MaxLength(100)]
        public string? VideoMimeType { get; set; }

        public int CategoryId { get; set; }
    }
}
