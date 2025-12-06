using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_VER2.Models;
using SIMS_VER2.Services;
using System.Security.Claims;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentProfileController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentProfileController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await _studentService.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Student student)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existingStudent = await _studentService.GetByUserIdAsync(userId);

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Allow updating Email, Phone, Address, Gender, DateOfBirth
            existingStudent.Email = student.Email;
            existingStudent.Phone = student.Phone;
            existingStudent.Address = student.Address;
            existingStudent.Gender = student.Gender;
            existingStudent.DateOfBirth = student.DateOfBirth;

            if (!ModelState.IsValid)
            {
                return View(existingStudent);
            }

            try
            {
                await _studentService.UpdateAsync(existingStudent);
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(existingStudent);
            }
        }
    }
}

