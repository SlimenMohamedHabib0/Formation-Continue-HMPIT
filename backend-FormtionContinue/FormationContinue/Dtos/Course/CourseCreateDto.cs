using System.ComponentModel.DataAnnotations;

namespace FormationContinue.Dtos.Course
{
    public class CourseCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string MotsCles { get; set; } = null!;

        [Required]
        public string CategoryName { get; set; } = null!;

    }
}
