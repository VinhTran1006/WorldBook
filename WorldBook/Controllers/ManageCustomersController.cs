using Microsoft.AspNetCore.Mvc;
using WorldBook.Services;
using WorldBook.ViewModels;
using WorldBook.Models;

namespace WorldBook.Controllers
{
    public class ManageCustomersController : Controller
    {
        private readonly IUserService _userService;

        public ManageCustomersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            var customers = await _userService.GetAllCustomersAsync();
            return View("/Views/AdminViews/ManageCustomer/Index.cshtml", customers);
            // 👉 Index nên return View thay vì Partial để nó load đầy đủ
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _userService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return PartialView("~/Views/AdminViews/ManageCustomer/Details.cshtml", customer);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _userService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return PartialView("~/Views/AdminViews/ManageCustomer/Delete.cshtml", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteCustomerAsync(id);
            return Json(new { success = true, message = "Customer đã được xóa (inactive)" });
        }
    }
}
