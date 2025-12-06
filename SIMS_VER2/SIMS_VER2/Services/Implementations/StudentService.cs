using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;

namespace SIMS_VER2.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetByStudentCodeAsync(string studentCode)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        }

        public async Task<Student?> GetByUserIdAsync(int userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            // Generate StudentCode if empty
            if (string.IsNullOrEmpty(student.StudentCode))
            {
                student.StudentCode = await GenerateNextStudentCodeAsync();
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateNextStudentCodeAsync()
        {
            var lastStudent = await _context.Students
                .Where(s => s.StudentCode.StartsWith("BD"))
                .OrderByDescending(s => s.StudentCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastStudent != null)
            {
                var lastCode = lastStudent.StudentCode;
                var numberPart = lastCode.Substring(2);
                if (int.TryParse(numberPart, out var n))
                {
                    nextNumber = n + 1;
                }
            }

            return $"BD{nextNumber:D5}";
        }
    }
}

