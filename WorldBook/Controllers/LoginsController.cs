using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        private readonly IRegisterService _registerService;
        private readonly IPasswordResetService _passwordResetService;
        public LoginsController(
           IAuthService authService,
           IGoogleAuthService googleAuthService,
           WorldBookDbContext db,
           IRegisterService registerService,
           IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _registerService = registerService;
            _db = db;
            _passwordResetService = passwordResetService;
        }

        [HttpGet]
        public IActionResult Login() => View("~/Views/Logins/Login.cshtml");

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Handle JSON request
            if (model != null && !string.IsNullOrEmpty(model.Username))
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Invalid input" });

                var user = await _authService.ValidateUserAsync(model.Username, model.Password);
                if (user == null)
                {
                    return Json(new { success = false, message = "Wrong username or password" });
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

                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
                string redirectUrl = roles.Contains("Customer")
                    ? Url.Action("GetBookHomePage", "Book")
                    : Url.Action("GetBookDashBoard", "Book");

                return Json(new
                {
                    success = true,
                    message = $"Welcome {user.Username}!",
                    redirectUrl = redirectUrl
                });
            }

            // Fallback: render view if not JSON
            return View("~/Views/Logins/Login.cshtml");
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

        [HttpGet]
        public IActionResult Register() => View("~/Views/Logins/Login.cshtml");

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var errorMessage = string.Join(", ", errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errorMessage });
            }

            try
            {
                await _registerService.RegisterUserAsync(model);
                return Json(new { success = true, message = "Registration successful! Please sign in." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckUsernameExists([FromBody] dynamic request)
        {
            try
            {
                string username = request.username;
                if (string.IsNullOrEmpty(username))
                    return Json(new { exists = false });

                var exists = await _registerService.UsernameExistsAsync(username);
                return Json(new { exists });
            }
            catch
            {
                return Json(new { exists = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailExists([FromBody] dynamic request)
        {
            try
            {
                string email = request.email;
                if (string.IsNullOrEmpty(email))
                    return Json(new { exists = false });

                var exists = await _registerService.EmailExistsAsync(email);
                return Json(new { exists });
            }
            catch
            {
                return Json(new { exists = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckPhoneExists([FromBody] dynamic request)
        {
            try
            {
                string phone = request.phone;
                if (string.IsNullOrEmpty(phone))
                    return Json(new { exists = false });

                var exists = await _registerService.PhoneExistsAsync(phone);
                return Json(new { exists });
            }
            catch
            {
                return Json(new { exists = false });
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
    => View("~/Views/Logins/ForgotPassword.cshtml");

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid email" });

            try
            {
                var result = await _passwordResetService.SendPasswordResetEmailAsync(model.Email);
                if (result)
                    return Json(new
                    {
                        success = true,
                        message = "A password reset link has been sent to your email. Please check your inbox (and spam)."
                    });

                return Json(new { success = false, message = "Email does not exist in the system" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var isValid = await _passwordResetService.ValidateResetTokenAsync(token);
            if (!isValid)
                return View("~/Views/Logins/ResetPasswordError.cshtml");

            return View("~/Views/Logins/ResetPassword.cshtml", new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            if (string.IsNullOrEmpty(model.Token))
                return Json(new { success = false, message = "Invalid token" });

            if (model.NewPassword != model.ConfirmPassword)
                return Json(new { success = false, message = "Passwords do not match" });

            if (model.NewPassword.Length < 6)
                return Json(new { success = false, message = "Password must be at least 6 characters" });

            try
            {
                var result = await _passwordResetService.ResetPasswordAsync(model.Token, model.NewPassword);
                if (result)
                    return Json(new
                    {
                        success = true,
                        message = "Password has been reset successfully. Please log in with the new password."
                    });

                return Json(new { success = false, message = "Token is invalid or expired" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
