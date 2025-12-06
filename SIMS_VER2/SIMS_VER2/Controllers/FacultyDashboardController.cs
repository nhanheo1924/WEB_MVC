using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_VER2.Services;
using System.Security.Claims;

namespace SIMS_VER2.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyDashboardController : Controller
    {
        private readonly ICourseOfferingService _offeringService;

        public FacultyDashboardController(ICourseOfferingService offeringService)
        {
            _offeringService = offeringService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var offerings = await _offeringService.GetByInstructorAsync(userId);
            return View(offerings);
        }
    }
}

