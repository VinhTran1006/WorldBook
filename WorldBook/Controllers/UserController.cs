using Microsoft.AspNetCore.Mvc;
using WorldBook.Services;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Profile
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Logins");

            var profile = await _userService.GetProfileAsync(username);
            if (profile == null) return NotFound();

            return View("~/Views/UserViews/Home/Profile.cshtml", profile);
        }

        // ✅ GET Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Logins");

            var profile = await _userService.GetProfileAsync(username);
            if (profile == null) return NotFound();

            return View(profile);
        }

        // ✅ POST Edit
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Logins");

            await _userService.UpdateProfileAsync(model, username);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

    }
}
