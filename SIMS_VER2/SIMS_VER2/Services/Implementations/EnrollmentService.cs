using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;

namespace SIMS_VER2.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByCourseOfferingIdAsync(int courseOfferingId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                .Where(e => e.CourseOfferingId == courseOfferingId)
                .ToListAsync();
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            if (!await CanEnrollAsync(enrollment.StudentId, enrollment.CourseOfferingId))
                throw new InvalidOperationException("Cannot enroll in this course offering.");

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanEnrollAsync(int studentId, int courseOfferingId)
        {
            // Check if already enrolled
            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseOfferingId == courseOfferingId);

            if (existing != null)
                return false;

            // Check if offering is full
            var offering = await _context.CourseOfferings
                .Include(co => co.Enrollments)
                .FirstOrDefaultAsync(co => co.Id == courseOfferingId);

            if (offering == null)
                return false;

            if (offering.Enrollments.Count >= offering.MaxStudents)
                return false;

            return true;
        }
    }
}

