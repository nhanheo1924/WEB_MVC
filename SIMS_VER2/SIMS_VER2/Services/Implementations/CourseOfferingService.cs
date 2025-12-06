using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;

namespace SIMS_VER2.Services.Implementations
{
    public class CourseOfferingService : ICourseOfferingService
    {
        private readonly ApplicationDbContext _context;

        public CourseOfferingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseOffering>> GetAllAsync()
        {
            return await _context.CourseOfferings
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .ToListAsync();
        }

        public async Task<CourseOffering?> GetByIdAsync(int id)
        {
            return await _context.CourseOfferings
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .FirstOrDefaultAsync(co => co.Id == id);
        }

        public async Task<IEnumerable<CourseOffering>> GetByInstructorAsync(int instructorUserId)
        {
            return await _context.CourseOfferings
                .Include(co => co.Course)
                .Where(co => co.InstructorUserId == instructorUserId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseOffering>> GetAvailableAsync(string? program = null)
        {
            var query = _context.CourseOfferings
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .Include(co => co.Enrollments)
                .Where(co => co.Status == "OPEN" || co.Status == "ONGOING");

            if (!string.IsNullOrEmpty(program))
            {
                query = query.Where(co => co.Course.Program == program || string.IsNullOrEmpty(co.Course.Program));
            }

            return await query.ToListAsync();
        }

        public async Task<CourseOffering> CreateAsync(CourseOffering offering)
        {
            _context.CourseOfferings.Add(offering);
            await _context.SaveChangesAsync();
            return offering;
        }

        public async Task<CourseOffering> UpdateAsync(CourseOffering offering)
        {
            _context.CourseOfferings.Update(offering);
            await _context.SaveChangesAsync();
            return offering;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var offering = await _context.CourseOfferings
                .Include(co => co.Enrollments)
                .FirstOrDefaultAsync(co => co.Id == id);

            if (offering == null)
                return false;

            // Check if offering has enrollments
            if (offering.Enrollments.Any())
                throw new InvalidOperationException("Cannot delete course offering that has enrollments.");

            _context.CourseOfferings.Remove(offering);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

