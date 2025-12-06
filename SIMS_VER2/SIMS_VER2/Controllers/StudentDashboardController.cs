using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_VER2.Services;
using System.Security.Claims;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseOfferingService _courseOfferingService;
        private readonly ILogger<StudentDashboardController> _logger;

        public StudentDashboardController(
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            ICourseOfferingService courseOfferingService,
            ILogger<StudentDashboardController> logger)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _courseOfferingService = courseOfferingService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID claim");
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                var student = await _studentService.GetByUserIdAsync(userId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                    TempData["Error"] = "Student profile not found. Please contact administrator.";
                    return RedirectToAction("AccessDenied", "Auth");
                }

                var currentEnrollments = await _enrollmentService.GetByStudentIdAsync(student.Id);
                var activeEnrollments = currentEnrollments.Where(e => e.Status == "ENROLLED").ToList();

                ViewBag.Student = student;
                ViewBag.CurrentEnrollments = activeEnrollments;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Dashboard action");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> MyCourses()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID claim");
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                var student = await _studentService.GetByUserIdAsync(userId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                    TempData["Error"] = "Student profile not found. Please contact administrator.";
                    return RedirectToAction("AccessDenied", "Auth");
                }

                var enrollments = await _enrollmentService.GetByStudentIdAsync(student.Id);
                ViewBag.Student = student;
                return View(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MyCourses action");
                TempData["Error"] = "An error occurred while loading your courses.";
                return RedirectToAction("Dashboard");
            }
        }

        public async Task<IActionResult> BrowseCourses()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID claim");
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                var student = await _studentService.GetByUserIdAsync(userId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                    TempData["Error"] = "Student profile not found. Please contact administrator.";
                    return RedirectToAction("AccessDenied", "Auth");
                }

                var availableOfferings = await _courseOfferingService.GetAvailableAsync(student.Program);
                var enrolledOfferings = (await _enrollmentService.GetByStudentIdAsync(student.Id))
                    .Select(e => e.CourseOfferingId)
                    .ToList();

                ViewBag.Student = student;
                ViewBag.EnrolledOfferings = enrolledOfferings;
                return View(availableOfferings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BrowseCourses action");
                TempData["Error"] = "An error occurred while loading available courses.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseOfferingId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(BrowseCourses));
            }

            try
            {
                var canEnroll = await _enrollmentService.CanEnrollAsync(student.Id, courseOfferingId);
                if (!canEnroll)
                {
                    TempData["Error"] = "Cannot enroll in this course. It may be full or you are already enrolled.";
                    return RedirectToAction(nameof(BrowseCourses));
                }

                var enrollment = new Models.Enrollment
                {
                    StudentId = student.Id,
                    CourseOfferingId = courseOfferingId,
                    Status = "ENROLLED"
                };

                await _enrollmentService.CreateAsync(enrollment);
                TempData["Success"] = "Successfully enrolled in the course!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error enrolling in course: {ex.Message}";
            }

            return RedirectToAction(nameof(BrowseCourses));
        }

        public async Task<IActionResult> Grades()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID claim");
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "Auth");
                }

                var student = await _studentService.GetByUserIdAsync(userId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found for user ID: {UserId}", userId);
                    TempData["Error"] = "Student profile not found. Please contact administrator.";
                    return RedirectToAction("AccessDenied", "Auth");
                }

                var enrollments = await _enrollmentService.GetByStudentIdAsync(student.Id);
                var enrollmentsWithGrades = enrollments
                    .Where(e => e.Grade.HasValue)
                    .OrderByDescending(e => e.CourseOffering.Year)
                    .ThenByDescending(e => e.CourseOffering.Semester)
                    .ToList();

                var allEnrollments = enrollments
                    .OrderByDescending(e => e.CourseOffering.Year)
                    .ThenByDescending(e => e.CourseOffering.Semester)
                    .ToList();

                ViewBag.Student = student;
                ViewBag.EnrollmentsWithGrades = enrollmentsWithGrades;
                return View(allEnrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Grades action");
                TempData["Error"] = "An error occurred while loading your grades.";
                return RedirectToAction("Dashboard");
            }
        }
    }
}

