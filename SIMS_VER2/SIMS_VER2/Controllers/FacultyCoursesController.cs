using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Services;
using System.Security.Claims;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyCoursesController : Controller
    {
        private readonly ICourseOfferingService _offeringService;
        private readonly ApplicationDbContext _context;

        public FacultyCoursesController(
            ICourseOfferingService offeringService,
            ApplicationDbContext context)
        {
            _offeringService = offeringService;
            _context = context;
        }

        public async Task<IActionResult> MyOfferings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var offerings = await _offeringService.GetByInstructorAsync(userId);
            return View(offerings);
        }

        [HttpGet]
        public async Task<IActionResult> OfferingStudents(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // Load offering with Course included
                var offering = await _context.CourseOfferings
                    .Include(co => co.Course)
                    .Include(co => co.Instructor)
                    .FirstOrDefaultAsync(co => co.Id == id);

                if (offering == null || offering.InstructorUserId != userId)
                {
                    TempData["Error"] = "You don't have permission to view this course offering.";
                    return RedirectToAction("MyOfferings");
                }

                // Load enrollments with all necessary includes
                var enrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.CourseOffering)
                        .ThenInclude(co => co.Course)
                    .Where(e => e.CourseOfferingId == id)
                    .OrderBy(e => e.Student.StudentCode)
                    .ToListAsync();

                ViewBag.Offering = offering;
                return View(enrollments);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("MyOfferings");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int enrollmentId, decimal? grade)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var enrollment = await _context.Enrollments
                    .Include(e => e.CourseOffering)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId);

                if (enrollment == null)
                {
                    TempData["Error"] = "Enrollment not found.";
                    return RedirectToAction("MyOfferings");
                }

                // Verify that the faculty is the instructor of this course offering
                if (enrollment.CourseOffering.InstructorUserId != userId)
                {
                    return Forbid();
                }

                // Validate grade range (0-10 or null)
                if (grade.HasValue && (grade < 0 || grade > 10))
                {
                    TempData["Error"] = "Grade must be between 0 and 10.";
                    return RedirectToAction("OfferingStudents", new { id = enrollment.CourseOfferingId });
                }

                enrollment.Grade = grade;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Grade updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating grade: {ex.Message}";
            }

            var enrollmentForRedirect = await _context.Enrollments.FindAsync(enrollmentId);
            return RedirectToAction("OfferingStudents", new { id = enrollmentForRedirect?.CourseOfferingId });
        }
    }
}

