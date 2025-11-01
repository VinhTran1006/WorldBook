using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WorldBook.Models;
using WorldBook.Services;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> UserFeedbackList(int? orderId = null, string filter = "all", int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);

            // Nếu có orderId → logic cũ (chỉ hiển thị feedback của order đó, không phân trang)
            if (orderId.HasValue)
            {
                var feedbacks = await _feedbackService.GetFeedbacksByOrderAsync(userId, orderId.Value);
                if (!feedbacks.Any())
                {
                    TempData["Info"] = "You haven't written feedback for this order yet.";
                    return RedirectToAction("OrderHistory", "Order");
                }

                var viewModel = new PaginatedFeedbackViewModel
                {
                    Feedbacks = feedbacks,
                    CurrentPage = 1,
                    TotalPages = 1,
                    CurrentFilter = "all",
                    CurrentOrderId = orderId,
                    TotalItems = feedbacks.Count()
                };

                return View("~/Views/UserViews/Feedback/UserFeedbackList.cshtml", viewModel);
            }

            // Nếu không có orderId → áp dụng filter và phân trang
            var paginatedResult = await _feedbackService.GetFilteredFeedbacksByUserAsync(userId, filter, page);

            if (paginatedResult.TotalItems == 0)
            {
                TempData["Info"] = filter == "written"
                    ? "You haven't written any feedback yet."
                    : "No feedback available.";
                return RedirectToAction("OrderHistory", "Order");
            }

            return View("~/Views/UserViews/Feedback/UserFeedbackList.cshtml", paginatedResult);
        }



        [HttpGet]
        public async Task<IActionResult> WriteFeedback(int orderId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);

            if (await _feedbackService.HasFeedbackAsync(orderId, userId))
            {
                TempData["WarningMessage"] = "You have already submitted feedback for this order.";
                return RedirectToAction("UserFeedbackList", new { orderId = orderId });
            }


            var books = await _feedbackService.GetBooksForFeedbackAsync(orderId);


            return View("~/Views/UserViews/Feedback/WriteFeedback.cshtml", books.ToList());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteFeedback(List<UserFeedbackViewModel> feedbacks)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);

            if (feedbacks == null || !feedbacks.Any())
                return RedirectToAction("UserFeedbackList");

            if (!ModelState.IsValid)
            {
                // Trả lại trang WriteFeedback với dữ liệu cũ
                return View("~/Views/UserViews/Feedback/WriteFeedback.cshtml", feedbacks);
            }

            foreach (var fb in feedbacks)
            {
                // Chỉ save những feedback có Star
                if (fb.Star.HasValue && fb.Star > 0)
                {
                    fb.UserId = userId;
                    fb.CreateAt = DateTime.Now;
                    await _feedbackService.AddFeedbackAsync(fb);
                }
            }

            TempData["SuccessMessage"] = "Thank you for your feedback!";
            return RedirectToAction("UserFeedbackList");
        }

        [HttpGet]
        public async Task<IActionResult> EditFeedback(int feedbackId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);
            var feedback = await _feedbackService.GetFeedbackByIdAsync(feedbackId);

            if (feedback == null || feedback.UserId != userId)
                return RedirectToAction("UserFeedbackList");

            return View("~/Views/UserViews/Feedback/EditFeedback.cshtml", feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeedback(UserFeedbackViewModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);
            var feedback = await _feedbackService.GetFeedbackByIdAsync(model.FeedbackId);

            if (feedback == null || feedback.UserId != userId)
                return RedirectToAction("UserFeedbackList");

            Console.WriteLine($"Star nhận được: {model.Star}");

            await _feedbackService.UpdateFeedbackAsync(model);
            TempData["SuccessMessage"] = "Feedback updated successfully!";
            return RedirectToAction("UserFeedbackList");
        }
    }
}
