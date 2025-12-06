using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_VER2.Models;
using SIMS_VER2.Services;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCoursesController : Controller
    {
        private readonly ICourseService _courseService;

        public AdminCoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var courses = await _courseService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c =>
                    c.CourseCode.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    c.CourseName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            try
            {
                await _courseService.CreateAsync(course);
                TempData["Success"] = "Course created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(course);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            try
            {
                await _courseService.UpdateAsync(course);
                TempData["Success"] = "Course updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(course);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _courseService.DeleteAsync(id);
                if (result)
                {
                    TempData["Success"] = "Course deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Course not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

