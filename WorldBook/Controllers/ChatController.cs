using Microsoft.AspNetCore.Mvc;
using WorldBook.Services;

namespace WorldBook.Controllers
{
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly GeminiService _geminiService;

        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        // ✅ Gợi ý câu hỏi khi mở chat
        [HttpGet("suggestions")]
        public IActionResult GetSuggestions()
        {
            var list = _geminiService.GetSuggestions();
            return Json(list);
        }

        // ✅ Xử lý người dùng gửi tin nhắn
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromForm] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "Bạn chưa nhập câu hỏi" });

            var reply = await _geminiService.AskGeminiAsync(message);
            return Json(new { reply });
        }

        [HttpGet("is-in-admin-chat")]
        public IActionResult IsInAdminChat()
        {
            var userName = User.Identity?.Name ?? "Khách";
            var isInChat = _geminiService.IsUserInAdminChat(userName);
            return Json(new { inAdminChat = isInChat });
        }
    }
}
