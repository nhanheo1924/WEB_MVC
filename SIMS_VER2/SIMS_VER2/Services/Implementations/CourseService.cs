using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;

namespace SIMS_VER2.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course> CreateAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseOfferings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return false;

            // Check if course has offerings
            if (course.CourseOfferings.Any())
                throw new InvalidOperationException("Cannot delete course that has course offerings.");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

