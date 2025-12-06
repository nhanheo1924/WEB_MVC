using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_VER2.Models;
using SIMS_VER2.Services;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStudentsController : Controller
    {
        private readonly IStudentService _studentService;

        public AdminStudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var students = await _studentService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s =>
                    s.StudentCode.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (s.Email != null && s.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            try
            {
                await _studentService.CreateAsync(student);
                TempData["Success"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(student);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            try
            {
                await _studentService.UpdateAsync(student);
                TempData["Success"] = "Student updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(student);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _studentService.DeleteAsync(id);
                if (result)
                {
                    TempData["Success"] = "Student deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Student not found.";
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

