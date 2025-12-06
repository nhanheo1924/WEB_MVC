using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS_VER2.Data;
using SIMS_VER2.Models;
using SIMS_VER2.Services;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCourseOfferingsController : Controller
    {
        private readonly ICourseOfferingService _offeringService;
        private readonly ICourseService _courseService;
        private readonly ApplicationDbContext _context;

        public AdminCourseOfferingsController(
            ICourseOfferingService offeringService,
            ICourseService courseService,
            ApplicationDbContext context)
        {
            _offeringService = offeringService;
            _courseService = courseService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? semester, int? year, int? courseId)
        {
            var offerings = await _offeringService.GetAllAsync();

            if (!string.IsNullOrEmpty(semester))
            {
                offerings = offerings.Where(o => o.Semester == semester).ToList();
            }

            if (year.HasValue)
            {
                offerings = offerings.Where(o => o.Year == year.Value).ToList();
            }

            if (courseId.HasValue)
            {
                offerings = offerings.Where(o => o.CourseId == courseId.Value).ToList();
            }

            ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName");
            return View(offerings);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName");
            ViewBag.Instructors = new SelectList(
                await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseOffering offering)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName");
                ViewBag.Instructors = new SelectList(
                    await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                    "Id", "Username");
                return View(offering);
            }

            try
            {
                await _offeringService.CreateAsync(offering);
                TempData["Success"] = "Course offering created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName");
                ViewBag.Instructors = new SelectList(
                    await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                    "Id", "Username");
                return View(offering);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var offering = await _offeringService.GetByIdAsync(id);
            if (offering == null)
            {
                return NotFound();
            }

            ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName", offering.CourseId);
            ViewBag.Instructors = new SelectList(
                await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                "Id", "Username", offering.InstructorUserId);
            return View(offering);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseOffering offering)
        {
            if (id != offering.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName", offering.CourseId);
                ViewBag.Instructors = new SelectList(
                    await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                    "Id", "Username", offering.InstructorUserId);
                return View(offering);
            }

            try
            {
                await _offeringService.UpdateAsync(offering);
                TempData["Success"] = "Course offering updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Courses = new SelectList(await _courseService.GetAllAsync(), "Id", "CourseName", offering.CourseId);
                ViewBag.Instructors = new SelectList(
                    await _context.Users.Where(u => u.Role == Role.Faculty && u.Active).ToListAsync(),
                    "Id", "Username", offering.InstructorUserId);
                return View(offering);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var offering = await _offeringService.GetByIdAsync(id);
            if (offering == null)
            {
                return NotFound();
            }
            return View(offering);
        }

        [HttpGet]
        public async Task<IActionResult> Students(int id)
        {
            var offering = await _offeringService.GetByIdAsync(id);
            if (offering == null)
            {
                return NotFound();
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseOfferingId == id)
                .ToListAsync();

            ViewBag.Offering = offering;
            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "Id", "FullName");
            return View(enrollments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int offeringId, int studentId)
        {
            try
            {
                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseOfferingId = offeringId,
                    Status = "ENROLLED"
                };

                await _context.Enrollments.AddAsync(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student added to class successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Students), new { id = offeringId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int offeringId, int enrollmentId)
        {
            try
            {
                var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
                if (enrollment != null)
                {
                    _context.Enrollments.Remove(enrollment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student removed from class successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Students), new { id = offeringId });
        }
    }
}

