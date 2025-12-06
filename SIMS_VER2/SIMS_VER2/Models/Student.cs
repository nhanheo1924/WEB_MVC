using System.ComponentModel.DataAnnotations;

namespace SIMS_VER2.Models
{
    public class Student
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string StudentCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string? Gender { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? Program { get; set; }
        
        [StringLength(20)]
        public string? Intake { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE";
        
        public int? UserId { get; set; }
        public User? User { get; set; }
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}

