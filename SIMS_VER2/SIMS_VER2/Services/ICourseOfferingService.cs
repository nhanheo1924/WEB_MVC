using SIMS_VER2.Models;

namespace SIMS_VER2.Services
{
    public interface ICourseOfferingService
    {
        Task<IEnumerable<CourseOffering>> GetAllAsync();
        Task<CourseOffering?> GetByIdAsync(int id);
        Task<IEnumerable<CourseOffering>> GetByInstructorAsync(int instructorUserId);
        Task<IEnumerable<CourseOffering>> GetAvailableAsync(string? program = null);
        Task<CourseOffering> CreateAsync(CourseOffering offering);
        Task<CourseOffering> UpdateAsync(CourseOffering offering);
        Task<bool> DeleteAsync(int id);
    }
}

