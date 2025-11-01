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

        private static HashSet<string> UsersInAdminChat = new();

        public GeminiService(IConfiguration configuration, WorldBookDbContext context)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _context = context;
        }

        // Đánh dấu user bắt đầu chat với admin
        public void StartAdminChat(string userName)
        {
            UsersInAdminChat.Add(userName);
        }

        // Kiểm tra user có đang chat với admin không
        public bool IsUserInAdminChat(string userName)
        {
            return UsersInAdminChat.Contains(userName);
        }

        // Xóa user khỏi chat admin (khi admin đóng cuộc chat)
        public void EndAdminChat(string userName)
        {
            UsersInAdminChat.Remove(userName);
        }

        public List<string> GetSuggestions()
        {
            return new List<string>
            {
                "Hôm nay có sách nào đang sale?",
                "Sách nào vừa mới được ra mắt?",
                "Cách đặt sách ở trang web?",
                "Giới thiệu về Website WorldBook Shop"
            };
        }

        // Hàm chính xử lý tin nhắn chat
        public async Task<string> AskGeminiAsync(string userMessage)
        {
            var message = userMessage.Trim().ToLower();

            // 1. Xử lý mấy câu cơ bản đầu tiên
            if (string.IsNullOrWhiteSpace(message))
                return "Bạn ơi, nhập câu hỏi trước nghen";

            if (message.Contains("sale"))
                return await GetSaleBooksAsync();

            if (message.Contains("mới") || message.Contains("ra mắt"))
                return await GetNewBooksAsync();

            if (message.Contains("đặt sách") || message.Contains("mua"))
                return "Để đặt sách, Bạn chỉ cần vào <b>Cart</b> rồi chọn thanh toán là xong nghen.";

            // 2. Nếu người dùng hỏi về website / giới thiệu
            if (IsAboutWebsite(message))
            {
                return @"
            <b>🌍 WorldBook Shop</b> là không gian mua sách online thân thiện dành cho những tâm hồn yêu đọc.<br/><br/>
            📚 Ở đây ní có thể thoải mái tìm kiếm hàng ngàn đầu sách đa dạng — từ tiểu thuyết, kỹ năng sống, đến sách chuyên ngành.<br/><br/>
            💳 Hỗ trợ nhiều hình thức thanh toán linh hoạt: online, chuyển khoản, hoặc thanh toán trực tiếp khi nhận hàng.<br/><br/>
            ❤️ Đặc biệt, WorldBook luôn cập nhật chương trình giảm giá, quà tặng và ưu đãi độc quyền cho bạn đọc trung thành.<br/><br/>
        ";
            }

            // 3. Nếu người dùng hỏi tóm tắt / review / nội dung sách
            if (IsSummaryRequest(message))
                return await HandleSummaryOrReviewAsync(userMessage);

            // 4. Nếu người dùng nhắc đến tên sách, tác giả hoặc thể loại trong DB
            var dbReply = await GetBookFromDatabaseAsync(message);
            if (!string.IsNullOrEmpty(dbReply))
                return dbReply;

            // 5. Không khớp gì hết → gọi Gemini API mặc định
            return await CallGeminiAPIAsync(userMessage);
        }

        // Xác định user đang hỏi tóm tắt / review
        private bool IsSummaryRequest(string message)
        {
            // Không bắt mấy từ "website", "worldbook" để tránh nhầm
            if (message.Contains("worldbook") || message.Contains("website"))
                return false;

            var keywords = new[] { "tóm tắt", "review", "đánh giá", "nội dung", "về sách", "về cuốn" };
            return keywords.Any(k => message.Contains(k));
        }

        // Xử lý yêu cầu tóm tắt / review
        private async Task<string> HandleSummaryOrReviewAsync(string userMessage)
        {
            var lower = userMessage.ToLower();

            var books = await _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .Where(b => b.IsActive)
                .ToListAsync();

            var foundBook = books.FirstOrDefault(b =>
                !string.IsNullOrEmpty(b.BookName) && lower.Contains(b.BookName.ToLower()));

            if (foundBook != null)
            {
                var authors = string.Join(", ", foundBook.BookAuthors.Select(a => a.Author.AuthorName));
                var categories = string.Join(", ", foundBook.BookCategories.Select(c => c.Category.CategoryName));

                // Lấy mô tả ngắn nếu có, nếu không thì nhờ Gemini tóm tắt giúp
                var summary = !string.IsNullOrEmpty(foundBook.BookDescription)
                    ? ShortenText(foundBook.BookDescription, 300)
                    : await CallGeminiAPIAsync($"Tóm tắt ngắn (2 câu) nội dung sách {foundBook.BookName}");

                // Gợi ý sách tương tự
                var suggestHtml = await GetSimilarBooksHtmlAsync(foundBook);

                return $@"
                    <b>{foundBook.BookName}</b><br/>
                    ✍️ {authors}<br/>
                    📂 {categories}<br/>
                    💬 {summary}<br/>
                    💰 Giá: {foundBook.BookPrice:N0} VND<br/>
                    <img src='{foundBook.ImageUrl1}' alt='book' width='100'/><br/>
                    <a href='/Book/GetBookDetails/{foundBook.BookId}'>Xem chi tiết / Mua ngay</a><br/>
                    {suggestHtml}
                ";
            }

            // Nếu không có trong DB → gọi Gemini để tóm tắt
            var geminiSummary = await CallGeminiAPIAsync($"Tóm tắt ngắn (2 câu) nội dung sách {userMessage}");
            var suggestRandom = await BuildSuggestionsHtmlAsync();

            return $"{geminiSummary}<br/><br/>Sách này hiện chưa có trong WorldBook Shop Bạn ơi.<br/>Bạn có thể xem mấy quyển tương tự nè:<br/>{suggestRandom}";
        }

        // Lấy danh sách sách tương tự theo thể loại
        private async Task<string> GetSimilarBooksHtmlAsync(Book foundBook)
        {
            var categoryId = foundBook.BookCategories.FirstOrDefault()?.CategoryId;

            if (categoryId == null)
                return await BuildSuggestionsHtmlAsync();

            var similarBooks = await _context.BookCategories
                .Where(bc => bc.CategoryId == categoryId && bc.BookId != foundBook.BookId)
                .Include(bc => bc.Book)
                .Select(bc => bc.Book)
                .Where(b => b.IsActive)
                .Take(3)
                .ToListAsync();

            if (!similarBooks.Any())
                return string.Empty;

            var list = string.Join("<br/>", similarBooks.Select(b =>
                $"• <a href='/Book/GetBookDetails/{b.BookId}'><b>{b.BookName}</b></a> - {b.BookPrice:N0} VND"));
            return $"<br/>Bạn có thể xem thêm mấy quyển tương tự nè:<br/>{list}";
        }

        // Gợi ý 3 quyển sách ngẫu nhiên (sale hoặc mới)
        private async Task<string> BuildSuggestionsHtmlAsync()
        {
            var books = await _context.Books
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.AddedAt)
                .Take(3)
                .ToListAsync();

            if (!books.Any())
                return "Hiện chưa có sách nào trong hệ thống.";

            var list = string.Join("<br/>", books.Select(b =>
                $"• <a href='/Book/GetBookDetails/{b.BookId}'><b>{b.BookName}</b></a> - {b.BookPrice:N0} VND"));
            return list;
        }

        // Tìm sách theo tên trong DB
        private async Task<string> GetBookFromDatabaseAsync(string message)
        {
            var books = await _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .Where(b => b.IsActive)
                .ToListAsync();

            var foundBook = books.FirstOrDefault(b =>
                !string.IsNullOrEmpty(b.BookName) && message.Contains(b.BookName.ToLower()));

            if (foundBook != null)
            {
                var authors = string.Join(", ", foundBook.BookAuthors.Select(a => a.Author.AuthorName));
                var categories = string.Join(", ", foundBook.BookCategories.Select(c => c.Category.CategoryName));
                var suggestHtml = await GetSimilarBooksHtmlAsync(foundBook);

                return $@"
                    <b>{foundBook.BookName}</b><br/>
                    💬 {ShortenText(foundBook.BookDescription, 300)}<br/>
                    💰 Giá: {foundBook.BookPrice:N0} VND<br/>
                    ✍️ {authors}<br/>
                    📂 {categories}<br/>
                    <img src='{foundBook.ImageUrl1}' alt='book' width='100'/><br/>
                    <a href='/Book/GetBookDetails/{foundBook.BookId}'>Xem chi tiết / Mua ngay</a><br/>
                    {suggestHtml}
                ";
            }

            return string.Empty;
        }

        // Danh sách sách đang sale
        private async Task<string> GetSaleBooksAsync()
        {
            var saleBooks = await _context.Books
                .Where(b => b.BookPrice < 100000 && b.IsActive)
                .Take(5)
                .ToListAsync();

            if (!saleBooks.Any())
                return "Hiện tại chưa có chương trình sale nào Bạn ơi.";

            var list = string.Join("<br/>", saleBooks.Select(b =>
                $"📘 <a href='/Book/GetBookDetails/{b.BookId}'><b>{b.BookName}</b></a> - {b.BookPrice:N0} VND"));
            return $"Dưới đây là vài cuốn đang sale nè Bạn ơi:<br/>{list}";
        }

        // Sách mới ra mắt
        private async Task<string> GetNewBooksAsync()
        {
            var newBooks = await _context.Books
                .OrderByDescending(b => b.AddedAt)
                .Take(5)
                .ToListAsync();

            var list = string.Join("<br/>", newBooks.Select(b =>
                $"✨ <a href='/Book/GetBookDetails/{b.BookId}'><b>{b.BookName}</b></a> - {b.BookPrice:N0} VND"));
            return $"Mấy quyển mới ra mắt gần đây nè bạn:<br/>{list}";
        }

        // Gọi Gemini API
        private async Task<string> CallGeminiAPIAsync(string userMessage)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/{model}:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Trả lời ngắn gọn, thân thiện, tự nhiên (tối đa 3 câu): {userMessage}" }
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

            return text ?? "AI hiện không thể phản hồi";
        }

        // Rút ngắn text (nếu mô tả quá dài)
        private string ShortenText(string? text, int maxChars)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (text.Length <= maxChars) return text;
            return text.Substring(0, maxChars).TrimEnd() + "...";
        }

        private bool IsAboutWebsite(string message)
        {
            var keywords = new[]
            {
        "giới thiệu worldbook",
        "về worldbook",
        "worldbook shop là gì",
        "giới thiệu về website",
        "worldbook là gì",
        "thông tin worldbook",
        "trang web worldbook",
        "giới thiệu về worldbook shop"
    };

            return keywords.Any(k => message.Contains(k));
        }
    }
}
