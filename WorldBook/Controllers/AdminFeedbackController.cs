using Microsoft.AspNetCore.Mvc;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    public class AdminFeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public AdminFeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: /AdminFeedback/Index
        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {
            int pageSize = 6;
            var pagedFeedbacks = await _feedbackService.GetPagedAdminFeedbacksAsync(page, pageSize, filter);

            ViewBag.CurrentFilter = filter;
            return View("~/Views/AdminViews/ManageFeedback/Index.cshtml", pagedFeedbacks);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var feedback = await _feedbackService.AdminGetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminViews/ManageFeedback/Detail.cshtml", feedback);
        }

        // Post reply
        [HttpPost]
        public async Task<IActionResult> Reply(int feedbackId, string replyContent)
        {
            int adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (string.IsNullOrWhiteSpace(replyContent))
            {
                TempData["Error"] = "Reply content cannot be empty.";
                return RedirectToAction("Detail", new { id = feedbackId });
            }

            await _feedbackService.ReplyToFeedbackAsync(feedbackId, adminId, replyContent);
            TempData["Success"] = "Reply sent successfully!";
            return RedirectToAction("Detail", new { id = feedbackId });
        }
    }
}
