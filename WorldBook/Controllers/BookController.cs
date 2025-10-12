using Microsoft.AspNetCore.Mvc;
using WorldBook.Services;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;
using WorldBook.ViewModels;

namespace WorldBook.Controllers
{
    public class BookController : Controller
    {
       private readonly IBookService _bookService;
         public BookController(IBookService bookService)
         {
              _bookService = bookService;
        }

        public async Task<IActionResult> GetBookHomePage()
        {
            var books = await _bookService.GetAllBooksAsync();
            BookViewModel bookViewModel = new BookViewModel
            {
                Books = books
            };
            return View("/Views/UserViews/Home/Index.cshtml", bookViewModel);
        }

        public async Task<IActionResult> GetBookDashBoard()
        {
            var books = await _bookService.GetAllBooksAsync();
            BookViewModel bookViewModel = new BookViewModel
            {
                Books = books
            };
            return View("/Views/AdminViews/ManageBook/View.cshtml", bookViewModel);
        }

        public async Task<IActionResult> GetBookDetails(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View("/Views/UserViews/Home/GetBookDetails.cshtml", book);
        }

        public async Task<IActionResult> GetBookDetailsDashBoard(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View("/Views/AdminViews/ManageBook/GetBookDetailsDashBoard.cshtml", book);
        }

        public IActionResult Add()
        {
            return PartialView("~/Views/AdminViews/ManageBook/Add.cshtml", new BookCreateEditViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(BookCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _bookService.AddBookAsync(model);
                return PartialView("~/Views/AdminViews/ManageBook/View.cshtml", model);

            }
            return PartialView("~/Views/AdminViews/ManageBook/Add.cshtml", model);
        }

    }
}
