using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Models;

namespace SIMS_VER2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Active).HasDefaultValue(true);
            });

            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.Property(e => e.StudentCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Program).HasMaxLength(50);
                entity.Property(e => e.Intake).HasMaxLength(20);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Student>(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CourseCode).IsUnique();
                entity.Property(e => e.CourseCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CourseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Credits).IsRequired();
                entity.Property(e => e.Program).HasMaxLength(50);
                entity.Property(e => e.Type).HasMaxLength(20);
            });

            // CourseOffering configuration
            modelBuilder.Entity<CourseOffering>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Semester).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ClassCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MaxStudents).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("OPEN");
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.CourseOfferings)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Instructor)
                    .WithMany()
                    .HasForeignKey(e => e.InstructorUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Enrollment configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("ENROLLED");
                entity.Property(e => e.Grade).HasColumnType("decimal(5,2)");
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CourseOffering)
                    .WithMany(co => co.Enrollments)
                    .HasForeignKey(e => e.CourseOfferingId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.StudentId, e.CourseOfferingId }).IsUnique();
            });
        }
    }
}

