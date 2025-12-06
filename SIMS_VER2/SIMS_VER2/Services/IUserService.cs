using SIMS_VER2.Models;
using SIMS_VER2.Models.ViewModels;

namespace SIMS_VER2.Services
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> RegisterStudentAsync(RegisterViewModel model);
        Task<bool> UsernameExistsAsync(string username);
    }
}

