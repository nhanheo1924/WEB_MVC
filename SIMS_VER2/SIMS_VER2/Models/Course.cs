using System.ComponentModel.DataAnnotations;

namespace SIMS_VER2.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int Credits { get; set; }
        
        [StringLength(50)]
        public string? Program { get; set; }
        
        [StringLength(20)]
        public string? Type { get; set; }
        
        public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
    }
}

