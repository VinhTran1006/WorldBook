using Microsoft.AspNetCore.Mvc;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View("~/Views/AdminViews/ManageOrder/Index.cshtml", orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View("~/Views/AdminViews/ManageOrder/Details.cshtml", order);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _orderService.ApproveNextStatusAsync(id);
                TempData["SuccessMessage"] = "Duyệt đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrderDetail(int id)
        {
            try
            {
                await _orderService.ApproveNextStatusAsync(id);
                TempData["SuccessMessage"] = "Duyệt đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrderDetai(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
