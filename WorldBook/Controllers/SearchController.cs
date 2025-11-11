using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class SearchController : Controller
    {
        private readonly WorldBookDbContext _context;

        public SearchController(WorldBookDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query, string filter = "all")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("GetBookHomePage", "Book");
            }

            var booksQuery = _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Supplier)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.IsActive);

            // Apply filter
            switch (filter.ToLower())
            {
                case "bookname":
                    booksQuery = booksQuery.Where(b =>
                        EF.Functions.Like(b.BookName, $"%{query}%"));
                    break;

                case "author":
                    booksQuery = booksQuery.Where(b =>
                        b.BookAuthors.Any(ba =>
                            EF.Functions.Like(ba.Author.AuthorName, $"%{query}%")));
                    break;

                case "category":
                    booksQuery = booksQuery.Where(b =>
                        b.BookCategories.Any(bc =>
                            EF.Functions.Like(bc.Category.CategoryName, $"%{query}%")));
                    break;

                case "publisher":
                    booksQuery = booksQuery.Where(b =>
                        EF.Functions.Like(b.Publisher.PublisherName, $"%{query}%"));
                    break;

                default: // "all"
                    booksQuery = booksQuery.Where(b =>
                        EF.Functions.Like(b.BookName, $"%{query}%") ||
                        b.BookAuthors.Any(ba =>
                            EF.Functions.Like(ba.Author.AuthorName, $"%{query}%")) ||
                        b.BookCategories.Any(bc =>
                            EF.Functions.Like(bc.Category.CategoryName, $"%{query}%")) ||
                        EF.Functions.Like(b.Publisher.PublisherName, $"%{query}%"));
                    break;
            }

            var books = await booksQuery.ToListAsync();

            var viewModel = new SearchResultViewModel
            {
                Query = query,
                Filter = filter,
                Books = books.Select(book => new BookDetailViewModel
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    BookDescription = book.BookDescription,
                    BookPrice = book.BookPrice,
                    BookQuantity = book.BookQuantity,
                    ImageUrl1 = book.ImageUrl1,
                    ImageUrl2 = book.ImageUrl2,
                    ImageUrl3 = book.ImageUrl3,
                    ImageUrl4 = book.ImageUrl4,
                    AddedAt = book.AddedAt,
                    PublisherName = book.Publisher?.PublisherName,
                    SupplierName = book.Supplier?.SupplierName,
                    AuthorNames = book.BookAuthors
                        .Where(ba => ba.Author != null)
                        .Select(ba => ba.Author.AuthorName)
                        .ToList(),
                    Categories = book.BookCategories
                        .Where(bc => bc.Category != null)
                        .Select(bc => bc.Category.CategoryName)
                        .ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<string>());
            }

            var suggestions = new List<string>();

            // Book names
            var bookNames = await _context.Books
                .Where(b => b.IsActive && EF.Functions.Like(b.BookName, $"%{term}%"))
                .Select(b => b.BookName)
                .Take(3)
                .ToListAsync();
            suggestions.AddRange(bookNames);

            // Author names
            var authorNames = await _context.Authors
                .Where(a => a.IsActive == true &&
                       EF.Functions.Like(a.AuthorName, $"%{term}%"))
                .Select(a => a.AuthorName)
                .Take(3)
                .ToListAsync();
            suggestions.AddRange(authorNames);

            // Category names
            var categoryNames = await _context.Categories
                .Where(c => c.IsActive == true &&
                       EF.Functions.Like(c.CategoryName, $"%{term}%"))
                .Select(c => c.CategoryName)
                .Take(2)
                .ToListAsync();
            suggestions.AddRange(categoryNames);

            return Json(suggestions.Distinct().Take(8));
        }
    }
}