using SIMS_VER2.Models;

namespace SIMS_VER2.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetByCourseOfferingIdAsync(int courseOfferingId);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(int id);
        Task<bool> CanEnrollAsync(int studentId, int courseOfferingId);
    }
}

