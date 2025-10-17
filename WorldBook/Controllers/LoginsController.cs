using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorldBook.Models;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class LoginsController : Controller
    {
        private readonly WorldBookDbContext _db;
        private readonly IAuthService _authService;
        public LoginsController(IAuthService authService) => _authService = authService;

        [HttpGet]
        public IActionResult Login() => View("~/Views/Logins/Login.cshtml");

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _authService.ValidateUserAsync(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Wrong password or user name");
                ViewData["LoginStatus"] = $"Login failed for user: {model.Username}";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            foreach (var r in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, r.Role.Name));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                "MyCookieAuth",
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            TempData["LoginSuccess"] = $"Welcome {user.Username}";

            TempData["Log"] = $"Username: {user.Username}\nEmail: {user.Email}\nID: {user.UserId}".Replace("\n", "<br/>");

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            if (roles.Contains("Customer"))
            {
                return View("~/Views/UserViews/Home/Index.cshtml");
            } else
            {
                return View("~/Views/AdminViews/DashBoards/DashBoard.cshtml");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            TempData["LogoutSuccess"] = "Logout successful!";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
