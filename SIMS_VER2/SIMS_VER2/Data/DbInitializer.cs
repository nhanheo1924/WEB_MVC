using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Models;
using System.Security.Cryptography;
using System.Text;

namespace SIMS_VER2.Data
{
    public static class DbInitializer
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Create Admin User
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = HashPassword("admin123"),
                Role = Role.Admin,
                Active = true
            };
            context.Users.Add(adminUser);

            // Create Faculty Users
            var faculty1 = new User
            {
                Username = "faculty1",
                PasswordHash = HashPassword("faculty123"),
                Role = Role.Faculty,
                Active = true
            };
            var faculty2 = new User
            {
                Username = "faculty2",
                PasswordHash = HashPassword("faculty123"),
                Role = Role.Faculty,
                Active = true
            };
            context.Users.AddRange(faculty1, faculty2);
            await context.SaveChangesAsync();

            // Create Students
            var students = new List<Student>
            {
                new Student
                {
                    StudentCode = "BD00001",
                    FullName = "John Doe",
                    DateOfBirth = new DateTime(2000, 1, 15),
                    Gender = "Male",
                    Email = "john.doe@student.edu",
                    Phone = "0123456789",
                    Address = "123 Main Street",
                    Program = "Computer Science",
                    Intake = "2020",
                    Status = "ACTIVE"
                },
                new Student
                {
                    StudentCode = "BD00002",
                    FullName = "Jane Smith",
                    DateOfBirth = new DateTime(2001, 3, 20),
                    Gender = "Female",
                    Email = "jane.smith@student.edu",
                    Phone = "0987654321",
                    Address = "456 Oak Avenue",
                    Program = "Computer Science",
                    Intake = "2020",
                    Status = "ACTIVE"
                },
                new Student
                {
                    StudentCode = "BD00003",
                    FullName = "Bob Johnson",
                    DateOfBirth = new DateTime(2000, 7, 10),
                    Gender = "Male",
                    Email = "bob.johnson@student.edu",
                    Phone = "0111222333",
                    Address = "789 Pine Road",
                    Program = "Information Technology",
                    Intake = "2021",
                    Status = "ACTIVE"
                },
                new Student
                {
                    StudentCode = "BD00004",
                    FullName = "Alice Williams",
                    DateOfBirth = new DateTime(2001, 11, 5),
                    Gender = "Female",
                    Email = "alice.williams@student.edu",
                    Phone = "0444555666",
                    Address = "321 Elm Street",
                    Program = "Information Technology",
                    Intake = "2021",
                    Status = "ACTIVE"
                },
                new Student
                {
                    StudentCode = "BD00005",
                    FullName = "Charlie Brown",
                    DateOfBirth = new DateTime(2000, 5, 25),
                    Gender = "Male",
                    Email = "charlie.brown@student.edu",
                    Phone = "0777888999",
                    Address = "654 Maple Drive",
                    Program = "Computer Science",
                    Intake = "2020",
                    Status = "ACTIVE"
                }
            };
            context.Students.AddRange(students);
            await context.SaveChangesAsync();

            // Create Student User for first student
            var studentUser = new User
            {
                Username = "student1",
                PasswordHash = HashPassword("student123"),
                Role = Role.Student,
                Active = true
            };
            context.Users.Add(studentUser);
            await context.SaveChangesAsync();

            // Link student to user
            students[0].UserId = studentUser.Id;
            await context.SaveChangesAsync();

            // Create Courses
            var courses = new List<Course>
            {
                new Course
                {
                    CourseCode = "CS101",
                    CourseName = "Introduction to Programming",
                    Description = "Basic programming concepts and fundamentals",
                    Credits = 3,
                    Program = "Computer Science",
                    Type = "CORE"
                },
                new Course
                {
                    CourseCode = "CS201",
                    CourseName = "Data Structures",
                    Description = "Study of data structures and algorithms",
                    Credits = 4,
                    Program = "Computer Science",
                    Type = "CORE"
                },
                new Course
                {
                    CourseCode = "CS301",
                    CourseName = "Database Systems",
                    Description = "Introduction to database design and SQL",
                    Credits = 3,
                    Program = "Computer Science",
                    Type = "CORE"
                },
                new Course
                {
                    CourseCode = "IT101",
                    CourseName = "Web Development",
                    Description = "Fundamentals of web development",
                    Credits = 3,
                    Program = "Information Technology",
                    Type = "CORE"
                },
                new Course
                {
                    CourseCode = "CS401",
                    CourseName = "Software Engineering",
                    Description = "Software development methodologies and practices",
                    Credits = 4,
                    Program = "Computer Science",
                    Type = "CORE"
                }
            };
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // Create Course Offerings
            var offerings = new List<CourseOffering>
            {
                new CourseOffering
                {
                    CourseId = courses[0].Id, // CS101
                    Semester = "Fall",
                    Year = 2024,
                    ClassCode = "CS101-F24-01",
                    InstructorUserId = faculty1.Id,
                    MaxStudents = 30,
                    Status = "ONGOING"
                },
                new CourseOffering
                {
                    CourseId = courses[1].Id, // CS201
                    Semester = "Fall",
                    Year = 2024,
                    ClassCode = "CS201-F24-01",
                    InstructorUserId = faculty1.Id,
                    MaxStudents = 25,
                    Status = "ONGOING"
                },
                new CourseOffering
                {
                    CourseId = courses[2].Id, // CS301
                    Semester = "Fall",
                    Year = 2024,
                    ClassCode = "CS301-F24-01",
                    InstructorUserId = faculty2.Id,
                    MaxStudents = 30,
                    Status = "ONGOING"
                },
                new CourseOffering
                {
                    CourseId = courses[3].Id, // IT101
                    Semester = "Fall",
                    Year = 2024,
                    ClassCode = "IT101-F24-01",
                    InstructorUserId = faculty2.Id,
                    MaxStudents = 35,
                    Status = "OPEN"
                },
                new CourseOffering
                {
                    CourseId = courses[4].Id, // CS401
                    Semester = "Spring",
                    Year = 2025,
                    ClassCode = "CS401-S25-01",
                    InstructorUserId = faculty1.Id,
                    MaxStudents = 20,
                    Status = "OPEN"
                }
            };
            context.CourseOfferings.AddRange(offerings);
            await context.SaveChangesAsync();

            // Create Enrollments
            var enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    StudentId = students[0].Id, // John Doe
                    CourseOfferingId = offerings[0].Id, // CS101
                    Status = "ENROLLED",
                    Grade = null
                },
                new Enrollment
                {
                    StudentId = students[0].Id, // John Doe
                    CourseOfferingId = offerings[1].Id, // CS201
                    Status = "ENROLLED",
                    Grade = null
                },
                new Enrollment
                {
                    StudentId = students[1].Id, // Jane Smith
                    CourseOfferingId = offerings[0].Id, // CS101
                    Status = "ENROLLED",
                    Grade = null
                },
                new Enrollment
                {
                    StudentId = students[1].Id, // Jane Smith
                    CourseOfferingId = offerings[2].Id, // CS301
                    Status = "ENROLLED",
                    Grade = null
                },
                new Enrollment
                {
                    StudentId = students[2].Id, // Bob Johnson
                    CourseOfferingId = offerings[3].Id, // IT101
                    Status = "ENROLLED",
                    Grade = null
                },
                new Enrollment
                {
                    StudentId = students[3].Id, // Alice Williams
                    CourseOfferingId = offerings[3].Id, // IT101
                    Status = "ENROLLED",
                    Grade = null
                }
            };
            context.Enrollments.AddRange(enrollments);
            await context.SaveChangesAsync();
        }
    }
}

