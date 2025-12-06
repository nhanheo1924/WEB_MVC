using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS_VER2.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        
        public int CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = null!;
        
        [StringLength(20)]
        public string Status { get; set; } = "ENROLLED";
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Grade { get; set; }
    }
}

