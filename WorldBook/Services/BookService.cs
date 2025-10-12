using WorldBook.Models;
using WorldBook.Repositories;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
        public class BookService : IBookService
        {
            private readonly CloudinaryService _cloudinaryService;
            private readonly IBookRepository _bookRepository;
            private readonly IPublisherRepository _publisherRepository;
            private readonly ISupplierRepository _supplierRepository;
            private readonly IAuthorRepository _authorRepository;
            private readonly IBookAuthorRepository _bookAuthorRepository;
            private readonly ICategoryRepository _categoryRepository;
            private readonly IBookCategoryRepository _bookCategoryRepository;

            public BookService(
                CloudinaryService cloudinaryService,
                IBookRepository bookRepository,
                IPublisherRepository publisherRepository,
                ISupplierRepository supplierRepository,
                IAuthorRepository authorRepository,
                IBookAuthorRepository bookAuthorRepository,
                ICategoryRepository categoryRepository,
                IBookCategoryRepository bookCategoryRepository)
            {
                _cloudinaryService = cloudinaryService;
                _bookRepository = bookRepository;
                _publisherRepository = publisherRepository;
                _supplierRepository = supplierRepository;
                _authorRepository = authorRepository;
                _bookAuthorRepository = bookAuthorRepository;
                _categoryRepository = categoryRepository;
                _bookCategoryRepository = bookCategoryRepository;
            }

            public async Task AddBookAsync(BookCreateEditViewModel vm)
            {
                // --- Publisher ---
                var publisher = await _publisherRepository.GetByNameAsync(vm.PublisherName);
                if (publisher == null)
                {
                    publisher = new Publisher { PublisherName = vm.PublisherName };
                    publisher = await _publisherRepository.AddAsync(publisher);
                }

                // --- Supplier ---
                var supplier = await _supplierRepository.GetByNameAsync(vm.SupplierName);
                if (supplier == null)
                {
                    supplier = new Supplier { SupplierName = vm.SupplierName };
                    supplier = await _supplierRepository.AddAsync(supplier);
                }

                // --- Upload Images lên Cloudinary ---
                string imageUrl1 = null;
                string imageUrl2 = null;
                string imageUrl3 = null;
                string imageUrl4 = null;

                if (vm.ImageUrl1 != null)
                    imageUrl1 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl1, "books")).SecureUrl.ToString();
                if (vm.ImageUrl2 != null)
                    imageUrl2 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl2, "books")).SecureUrl.ToString();
                if (vm.ImageUrl3 != null)
                    imageUrl3 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl3, "books")).SecureUrl.ToString();
                if (vm.ImageUrl4 != null)
                    imageUrl4 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl4, "books")).SecureUrl.ToString();

                // --- Tạo Book ---
                var book = new Book
                {
                    BookName = vm.BookName,
                    BookDescription = vm.BookDescription,
                    BookPrice = vm.BookPrice,
                    BookQuantity = vm.BookQuantity,
                    ImageUrl1 = imageUrl1,
                    ImageUrl2 = imageUrl2,
                    ImageUrl3 = imageUrl3,
                    ImageUrl4 = imageUrl4,
                    AddedAt = DateTime.Now,
                    PublisherId = publisher.PublisherId,
                    SupplierId = supplier.SupplierId
                };

                await _bookRepository.AddBookAsync(book);

                // --- Authors ---
                if (vm.AuthorNames != null && vm.AuthorNames.Any())
                {
                    foreach (var authorName in vm.AuthorNames)
                    {
                        var author = await _authorRepository.GetByNameAsync(authorName);
                        if (author == null)
                            author = await _authorRepository.AddAsync(new Author { AuthorName = authorName });

                        await _bookAuthorRepository.AddBookAuthorAsync(new BookAuthor
                        {
                            BookId = book.BookId,
                            AuthorId = author.AuthorId
                        });
                    }
                }

                // --- Categories ---
                if (vm.CategoryNames != null && vm.CategoryNames.Any())
                {
                    foreach (var categoryName in vm.CategoryNames)
                    {
                        var category = await _categoryRepository.GetByNameAsync(categoryName);
                        if (category == null)
                            category = await _categoryRepository.AddAsync(new Category { CategoryName = categoryName });

                        await _bookCategoryRepository.AddBookCategoryAsync(new BookCategory
                        {
                            BookId = book.BookId,
                            CategoryId = category.CategoryId
                        });
                    }
                }
            }


        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteBookAsync(id);
        }

        public async Task<IEnumerable<BookDetailViewModel>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();

            // map từng Book sang ViewModel
            return books.Select(book => new BookDetailViewModel
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
            });
        }

        public async Task<BookDetailViewModel> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) return null;

            return new BookDetailViewModel
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookDescription = book.BookDescription,
                BookPrice = book.BookPrice,
                BookQuantity = book.BookQuantity,
                ImageUrl1 = book.ImageUrl1,
                AddedAt = book.AddedAt,
                PublisherName = book.Publisher?.PublisherName,
                SupplierName = book.Supplier?.SupplierName,
                AuthorNames = book.BookAuthors.Select(ba => ba.Author.AuthorName).ToList(),
                 Categories = book.BookCategories.Select(ba => ba.Category.CategoryName).ToList()

            };
        }

        public async Task UpdateBookAsync(BookCreateEditViewModel book)
        {
            
        }
    }
}
