using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS_VER2.Models
{
    public class CourseOffering
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        [Required]
        [StringLength(20)]
        public string Semester { get; set; } = string.Empty;
        
        public int Year { get; set; }
        
        [Required]
        [StringLength(20)]
        public string ClassCode { get; set; } = string.Empty;
        
        public int? InstructorUserId { get; set; }
        
        [ForeignKey("InstructorUserId")]
        public User? Instructor { get; set; }
        
        public int MaxStudents { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "OPEN";
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}

