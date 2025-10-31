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
        public async Task<IActionResult> UserFeedbackList(int? orderId = null)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Logins");

            int userId = int.Parse(userIdClaim);

            IEnumerable<UserFeedbackViewModel> feedbacks;

            if (orderId.HasValue)
            {
                // Nếu có orderId → lấy feedback của order đó
                feedbacks = await _feedbackService.GetFeedbacksByOrderAsync(userId, orderId.Value);

                if (!feedbacks.Any())
                {
                    TempData["Info"] = "You haven't written feedback for this order yet.";
                    return RedirectToAction("OrderHistory", "Order");
                }
            }
            else
            {
                // Nếu không có orderId → lấy toàn bộ feedback của user
                feedbacks = await _feedbackService.GetFeedbacksByUserAsync(userId);

                if (!feedbacks.Any())
                {
                    TempData["Info"] = "You haven't written any feedback yet.";
                    return RedirectToAction("OrderHistory", "Order");
                }
            }

            return View("~/Views/UserViews/Feedback/UserFeedbackList.cshtml", feedbacks);
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

            foreach (var fb in feedbacks)
            {
                fb.UserId = userId;
                fb.CreateAt = DateTime.Now;
                await _feedbackService.AddFeedbackAsync(fb);
            }

            TempData["SuccessMessage"] = "Thank you for your feedback!";
            return RedirectToAction("UserFeedbackList");
        }
    }
}
