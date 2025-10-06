using Microsoft.AspNetCore.Mvc;
using WorldBook.Services;
using WorldBook.ViewModels;
using WorldBook.Models;

namespace WorldBook.Controllers
{
    public class ManageStaffController : Controller
    {
        private readonly IUserService _userService;

        public ManageStaffController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            var staffs = await _userService.GetAllStaffsAsync();
            return View("/Views/AdminViews/ManageStaff/Index.cshtml", staffs);
            // 👉 Index nên return View thay vì Partial để nó load đầy đủ
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var staff = await _userService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return PartialView("~/Views/AdminViews/ManageStaff/Details.cshtml", staff);
        }

        // GET: ManageStaff/Create
        public IActionResult Create()
        {
            return PartialView("~/Views/AdminViews/ManageStaff/Create.cshtml", new UserCreateViewModel());
        }

        // POST: ManageStaff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("~/Views/AdminViews/ManageStaff/Create.cshtml", vm);

            try
            {
                await _userService.CreateStaffAsync(vm);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView("~/Views/AdminViews/ManageStaff/Create.cshtml", vm);
            }
        }

        // GET: ManageStaff/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _userService.GetByIdAsync(id);
            if (staff == null) return NotFound();

            var vm = new UserEditViewModel
            {
                UserId = staff.UserId,
                Name = staff.Name,
                Phone = staff.Phone,
                Address = staff.Address,
                DateOfBirth = staff.DateOfBirth,
                Gender = staff.Gender,
                Email = staff.Email,
                IsActive = staff.IsActive
            };
            return PartialView("~/Views/AdminViews/ManageStaff/Edit.cshtml", vm);
        }

        // POST: ManageStaff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("~/Views/AdminViews/ManageStaff/Edit.cshtml", vm);

            try
            {
                await _userService.UpdateStaffAsync(vm.UserId, vm);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView("~/Views/AdminViews/ManageStaff/Edit.cshtml", vm);
            }
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _userService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return PartialView("~/Views/AdminViews/ManageStaff/Delete.cshtml", staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteStaffAsync(id);
            return Json(new { success = true, message = "Staff đã được xóa (inactive)" });
        }
    }
}
