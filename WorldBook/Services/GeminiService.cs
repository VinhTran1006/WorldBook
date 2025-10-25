using System.Text.Json;
using System.Text;
using WorldBook.Models;
using Microsoft.EntityFrameworkCore;

namespace WorldBook.Services
{
    public class GeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly WorldBookDbContext _context;

        public GeminiService(IConfiguration configuration, WorldBookDbContext context)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _context = context;
        }

        public List<string> GetSuggestions()
        {
            return new List<string>
            {
                "Hôm nay có sách nào đang sale?",
                "Sách nào vừa mới được ra mắt?",
                "Cách đặt sách ở trang web?"
            };
        }

        // ✅ Xử lý chat chính
        public async Task<string> AskGeminiAsync(string userMessage)
        {
            var message = userMessage.Trim().ToLower();

            // 1️⃣ — Nếu người dùng chọn gợi ý hoặc câu hỏi thông thường
            if (message.Contains("sale"))
                return await GetSaleBooksAsync();

            if (message.Contains("mới") || message.Contains("ra mắt"))
                return await GetNewBooksAsync();

            if (message.Contains("đặt sách") || message.Contains("mua"))
                return "Để đặt sách, ní chỉ cần vào <b>Cart</b> rồi chọn thanh toán là xong nghen ❤️.";

            // 2️⃣ — Nếu người dùng hỏi về tên sách, tác giả, thể loại
            var dbReply = await GetBookFromDatabaseAsync(message);
            if (!string.IsNullOrEmpty(dbReply))
                return dbReply;

            // 3️⃣ — Nếu không khớp, gọi Gemini như cũ
            return await CallGeminiAPIAsync(userMessage);
        }

        // 🔹 Truy xuất sách đang sale
        private async Task<string> GetSaleBooksAsync()
        {
            var saleBooks = await _context.Books
                .Where(b => b.BookPrice < 100000 && b.IsActive)
                .Take(5)
                .ToListAsync();

            if (!saleBooks.Any())
                return "Hiện tại chưa có chương trình sale nào ní ơi 😢.";

            var list = string.Join("<br/>", saleBooks.Select(b =>
                $"📘 <b>{b.BookName}</b> - {b.BookPrice:N0} VND"));
            return $"Dưới đây là vài cuốn đang sale nè ní ơi 😍:<br/>{list}";
        }

        // 🔹 Sách mới ra mắt
        private async Task<string> GetNewBooksAsync()
        {
            var newBooks = await _context.Books
                .OrderByDescending(b => b.AddedAt)
                .Take(5)
                .ToListAsync();

            var list = string.Join("<br/>", newBooks.Select(b =>
                $"✨ <b>{b.BookName}</b> - {b.BookPrice:N0} VND"));
            return $"Mấy quyển mới ra mắt gần đây nè ní 😍:<br/>{list}";
        }

        // 🔹 Kiểm tra trong DB xem có cuốn sách nào được nhắc đến không
        private async Task<string> GetBookFromDatabaseAsync(string message)
        {
            var books = await _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .ToListAsync();

            var foundBook = books.FirstOrDefault(b =>
                message.Contains(b.BookName.ToLower()));

            if (foundBook != null)
            {
                var authors = string.Join(", ", foundBook.BookAuthors.Select(a => a.Author.AuthorName));
                var categories = string.Join(", ", foundBook.BookCategories.Select(c => c.Category.CategoryName));

                // 🔸 Gợi ý thêm sách cùng thể loại
                var similarBooks = await _context.BookCategories
                    .Where(bc => bc.CategoryId == foundBook.BookCategories.FirstOrDefault().CategoryId
                              && bc.BookId != foundBook.BookId)
                    .Include(bc => bc.Book)
                    .Take(3)
                    .Select(bc => bc.Book)
                    .ToListAsync();

                var suggestText = "";
                if (similarBooks.Any())
                {
                    suggestText = "<br/><br/>📚 Ní có thể tham khảo thêm mấy quyển tương tự nè:<br/>" +
                        string.Join("<br/>", similarBooks.Select(b => $"• <b>{b.BookName}</b> - {b.BookPrice:N0} VND"));
                }

                return $@"
                    <b>{foundBook.BookName}</b><br/>
                    💬 {foundBook.BookDescription}<br/>
                    💰 Giá: {foundBook.BookPrice:N0} VND<br/>
                    ✍️ Tác giả: {authors}<br/>
                    📂 Thể loại: {categories}<br/>
                    <img src='{foundBook.ImageUrl1}' alt='book' width='100'/><br/>
                    👉 Ní có muốn tui thêm vô giỏ hàng hông? 😍
                    {suggestText}
                ";
            }

            return string.Empty;
        }

        // 🔹 Gọi Gemini API gốc
        private async Task<string> CallGeminiAPIAsync(string userMessage)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/{model}:generateContent?key={apiKey}";

            // ✅ Prompt ràng buộc trả lời ngắn
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new {
                        text = $"Trả lời ngắn gọn, tự nhiên, vui vẻ (2-3 câu tối đa): {userMessage}"
                    }
                }
            }
        }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Lỗi API: {response.StatusCode}";

            using var doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "AI hiện không thể phản hồi 😔";
        }

    }
}
