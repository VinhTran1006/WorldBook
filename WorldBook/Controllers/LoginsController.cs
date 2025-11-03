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
        private readonly IGoogleAuthService _googleAuthService;
        public LoginsController(
           IAuthService authService,
           IGoogleAuthService googleAuthService,
           WorldBookDbContext db)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _db = db;
        }

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
                return RedirectToAction("GetBookHomePage", "Book");
            } else
            {
                return RedirectToAction("GetBookDashBoard", "Book");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            TempData["LogoutSuccess"] = "Logout successful!";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Logins");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync("MyCookieAuth");

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Google login failed");
                return RedirectToAction("Login");
            }

            var emailClaim = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(emailClaim))
            {
                ModelState.AddModelError("", "Could not retrieve email from Google");
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy hoặc tạo user - user này đã có đầy đủ roles
                var user = await _googleAuthService.GetOrCreateUserFromGoogleAsync(emailClaim, nameClaim);

                // Sign out Google authentication
                await HttpContext.SignOutAsync("MyCookieAuth");

                // Sign in với user từ DB
                await SignInUserAsync(user);

                return RedirectByRole(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login error: {ex.Message}");
                return RedirectToAction("Login");
            }
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Thêm roles từ user.UserRoles
            if (user.UserRoles != null && user.UserRoles.Count > 0)
            {
                foreach (var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                }
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                "MyCookieAuth",
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });
        }

        private IActionResult RedirectByRole(User user)
        {
            if (user?.UserRoles == null || user.UserRoles.Count == 0)
            {
                return RedirectToAction("Login");
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            if (roles.Contains("Customer"))
            {
                return RedirectToAction("GetBookHomePage", "Book");
            }
            else if (roles.Contains("Admin") || roles.Contains("Staff"))
            {
                return RedirectToAction("GetBookDashBoard", "Book");
            }

            return RedirectToAction("Login");
        }
    }
}
