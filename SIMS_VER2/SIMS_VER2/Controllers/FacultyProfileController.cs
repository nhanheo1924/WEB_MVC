using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Services;
using System.Security.Claims;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<FacultyProfileController> _logger;

        public FacultyProfileController(
            ApplicationDbContext context,
            IUserService userService,
            ILogger<FacultyProfileController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading faculty profile");
                TempData["Error"] = "An error occurred while loading your profile.";
                return RedirectToAction("Dashboard", "FacultyDashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Models.User user)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                if (user.Id != userId)
                {
                    return Forbid();
                }

                var existingUser = await _userService.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Only allow updating username (if not already taken by another user)
                if (!string.IsNullOrEmpty(user.Username) && user.Username != existingUser.Username)
                {
                    var usernameExists = await _userService.UsernameExistsAsync(user.Username);
                    if (usernameExists)
                    {
                        ModelState.AddModelError("Username", "This username is already taken.");
                        return View(existingUser);
                    }
                    existingUser.Username = user.Username;
                }

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profile updated successfully. Please login again with your new username if changed.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating faculty profile");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out var userId))
                {
                    var existingUser = await _userService.GetByIdAsync(userId);
                    return View(existingUser);
                }
                return RedirectToAction("Dashboard", "FacultyDashboard");
            }
        }
    }
}

