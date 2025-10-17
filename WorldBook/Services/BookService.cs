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
            Console.WriteLine("=== vao service ===");
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

            Console.WriteLine("=== BẮT ĐẦU UPLOAD ẢNH ===");
            Console.WriteLine($"Ảnh 1: {(vm.ImageUrl1 != null ? vm.ImageUrl1.FileName : "null")}");
            Console.WriteLine($"Ảnh 2: {(vm.ImageUrl2 != null ? vm.ImageUrl2.FileName : "null")}");
            Console.WriteLine($"Ảnh 3: {(vm.ImageUrl3 != null ? vm.ImageUrl3.FileName : "null")}");
            Console.WriteLine($"Ảnh 4: {(vm.ImageUrl4 != null ? vm.ImageUrl4.FileName : "null")}");
            try
            {
                if (vm.ImageUrl1 != null)
                {
                    Console.WriteLine("⏳ Upload ảnh 1 lên Cloudinary...");
                    imageUrl1 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl1, "books")).SecureUrl.ToString();
                    Console.WriteLine($"✅ Upload ảnh 1 xong: {imageUrl1}");
                }

                if (vm.ImageUrl2 != null)
                {
                    Console.WriteLine("⏳ Upload ảnh 2 lên Cloudinary...");
                    imageUrl2 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl2, "books")).SecureUrl.ToString();
                    Console.WriteLine($"✅ Upload ảnh 2 xong: {imageUrl2}");
                }

                if (vm.ImageUrl3 != null)
                {
                    Console.WriteLine("⏳ Upload ảnh 3 lên Cloudinary...");
                    imageUrl3 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl3, "books")).SecureUrl.ToString();
                    Console.WriteLine($"✅ Upload ảnh 3 xong: {imageUrl3}");
                }

                if (vm.ImageUrl4 != null)
                {
                    Console.WriteLine("⏳ Upload ảnh 4 lên Cloudinary...");
                    imageUrl4 = (await _cloudinaryService.UploadImageAsync(vm.ImageUrl4, "books")).SecureUrl.ToString();
                    Console.WriteLine($"✅ Upload ảnh 4 xong: {imageUrl4}");
                }

                Console.WriteLine("=== HOÀN TẤT UPLOAD ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi upload ảnh: {ex.Message}");
                throw;
            }

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
                    IsActive = true,
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
