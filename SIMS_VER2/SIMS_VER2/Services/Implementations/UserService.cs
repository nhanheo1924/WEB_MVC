using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;
using SIMS_VER2.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace SIMS_VER2.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Active);

            if (user == null)
                return null;

            var passwordHash = HashPassword(password);
            
            // Trim and compare hashes (handle any whitespace issues)
            var dbHash = user.PasswordHash?.Trim();
            var computedHash = passwordHash.Trim();
            
            if (dbHash != computedHash)
            {
                // Hash mismatch - return null
                return null;
            }

            return user;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> RegisterStudentAsync(RegisterViewModel model)
        {
            // Check if username exists
            if (await UsernameExistsAsync(model.Username))
                return false;

            // Check if student exists and not linked to a user
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == model.StudentCode && s.UserId == null);

            if (student == null)
                return false;

            // Create user
            var user = new User
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password),
                Role = Role.Student,
                Active = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Link student to user
            student.UserId = user.Id;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

