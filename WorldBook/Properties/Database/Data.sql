-- Thêm Role
INSERT INTO [Role] (Name) VALUES
(N'Admin'),
(N'Employee'),
(N'Customer');
GO

-- Thêm User
INSERT INTO [User] (Username, DateOfBirth, Gender, Address, Name, Email, Phone, Password)
VALUES
-- Admin
(N'admin01', '1990-05-12', N'Male',   N'123 A Street, Hanoi', N'Nguyen Van Admin',
 N'admin@book.com', '0901234567',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),
-- Employee
(N'emp01', '1995-03-22', N'Female', N'45 B Street, HCMC', N'Tran Thi Employee',
 N'employee@book.com', '0912345678',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),
-- Customer
(N'cus01', '2000-11-01', N'Male',   N'78 C Street, Da Nang', N'Le Van Customer',
 N'customer@book.com', '0923456789',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),
(N'cus02', '1998-07-15', N'Female', N'100 D Street, Can Tho', N'Tran Bich Hanh',
 N'hanh.tran@gmail.com', '0987654321',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),
(N'cus03', '1992-01-30', N'Male', N'200 E Street, Hai Phong', N'Pham Minh Tuan',
 N'tuan.pham@yahoo.com', '0977112233',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92');
GO

-- Gán Role cho User
INSERT INTO [UserRole] (UserId, RoleId)
VALUES
( (SELECT UserId FROM [User] WHERE Username = 'admin01'), (SELECT RoleId FROM [Role] WHERE Name = 'Admin') ),
( (SELECT UserId FROM [User] WHERE Username = 'emp01'), (SELECT RoleId FROM [Role] WHERE Name = 'Employee') ),
( (SELECT UserId FROM [User] WHERE Username = 'cus01'), (SELECT RoleId FROM [Role] WHERE Name = 'Customer') ),
( (SELECT UserId FROM [User] WHERE Username = 'cus02'), (SELECT RoleId FROM [Role] WHERE Name = 'Customer') ),
( (SELECT UserId FROM [User] WHERE Username = 'cus03'), (SELECT RoleId FROM [Role] WHERE Name = 'Customer') );
GO

-- Thêm Nhà xuất bản (Publisher)
INSERT INTO Publisher (PublisherName, IsActive)
VALUES
(N'Tre Publishing House', 1),
(N'Kim Dong Publishing House', 1),
(N'Education Publishing House', 1),
(N'Addison-Wesley', 1),
(N'O''Reilly Media', 1),
(N'Penguin Books', 1),
(N'HarperCollins', 1);
GO

-- Thêm Nhà cung cấp (Supplier)
INSERT INTO Supplier (SupplierName, SupplierEmail, PhoneNumber, IsActive, ContactPerson, Address)
VALUES
(N'Fahasa', 'contact@fahasa.com', '1900636467', 1, N'Nguyen Van A', N'60-62 Le Loi, Dist. 1, HCMC'),
(N'Phuong Nam Book', 'contact@phuongnambook.com', '19006656', 1, N'Tran Thi B', N'09 Nguyen Thi Minh Khai, Dist. 1, HCMC'),
(N'Tiki Trading', 'trading@tiki.vn', '19006035', 1, N'Pham Van C', N'52 Ut Tich, Tan Binh Dist., HCMC'),
(N'Amazon US', 'support@amazon.com', '123456789', 1, N'John Doe', N'Seattle, WA, USA');
GO

-- Thêm Thể loại (Category)
INSERT INTO Category (CategoryName, CategoryDescription, IsActive)
VALUES
(N'Programming', N'Books on programming, algorithms, and software.', 1),
(N'Business & Economics', N'Books on business, management, finance.', 1),
(N'Literature', N'Novels, short stories, poetry.', 1),
(N'Self-Help', N'Books for personal development, soft skills.', 1),
(N'Children''s Books', N'Comics, educational books for children.', 1),
(N'Science & Technology', N'Specialized books on science and technology.', 1),
(N'Biography', N'Biographies and autobiographies.', 1),
(N'Fantasy & Sci-Fi', N'Science fiction and fantasy novels.', 1);
GO

-- Thêm Tác giả (Author)
INSERT INTO Author (AuthorName, AuthorDescription, IsActive)
VALUES
(N'Robert C. Martin', N'Known as "Uncle Bob", expert in Clean Code.', 1),
(N'Martin Fowler', N'Author of "Refactoring" and expert in software architecture.', 1),
(N'Erich Gamma', N'One of the "Gang of Four" (GoF) authors of "Design Patterns".', 1),
(N'Nguyen Nhat Anh', N'Famous Vietnamese author of youth novels.', 1),
(N'Dale Carnegie', N'Author of "How to Win Friends and Influence People".', 1),
(N'Thomas H. Cormen', N'Co-author of "Introduction to Algorithms" (CLRS).', 1),
(N'Yuval Noah Harari', N'Author of "Sapiens: A Brief History of Humankind".', 1),
(N'Fred Brooks', N'Author of "The Mythical Man-Month".', 1),
(N'Steve McConnell', N'Author of "Code Complete 2".', 1),
(N'Eric Freeman', N'Co-author of "Head First Design Patterns".', 1),
(N'Gayle Laakmann McDowell', N'Author of "Cracking the Coding Interview".', 1),
(N'Robert Kiyosaki', N'Author of "Rich Dad Poor Dad".', 1),
(N'Benjamin Graham', N'Author of "The Intelligent Investor".', 1),
(N'Napoleon Hill', N'Author of "Think and Grow Rich".', 1),
(N'Steven D. Levitt', N'Co-author of "Freakonomics".', 1),
(N'Peter Thiel', N'Author of "Zero to One".', 1),
(N'George Orwell', N'Author of "1984" and "Animal Farm".', 1),
(N'F. Scott Fitzgerald', N'Author of "The Great Gatsby".', 1),
(N'Harper Lee', N'Author of "To Kill a Mockingbird".', 1),
(N'J.D. Salinger', N'Author of "The Catcher in the Rye".', 1),
(N'Jane Austen', N'Author of "Pride and Prejudice".', 1),
(N'J.R.R. Tolkien', N'Author of "The Lord of the Rings".', 1),
(N'J.K. Rowling', N'Author of the "Harry Potter" series.', 1),
(N'Paulo Coelho', N'Author of "The Alchemist".', 1),
(N'Haruki Murakami', N'Author of "Norwegian Wood".', 1),
(N'Margaret Mitchell', N'Author of "Gone with the Wind".', 1),
(N'Stephen Covey', N'Author of "The 7 Habits of Highly Effective People".', 1),
(N'James Clear', N'Author of "Atomic Habits".', 1),
(N'Charles Duhigg', N'Author of "The Power of Habit".', 1),
(N'Viktor Frankl', N'Author of "Man''s Search for Meaning".', 1),
(N'Simon Sinek', N'Author of "Start with Why".', 1),
(N'Antoine de Saint-Exupéry', N'Author of "The Little Prince".', 1),
(N'E.B. White', N'Author of "Charlotte''s Web".', 1),
(N'Maurice Sendak', N'Author of "Where the Wild Things Are".', 1),
(N'Fujiko F. Fujio', N'Author of "Doraemon".', 1),
(N'Dr. Seuss', N'Author of "The Cat in the Hat".', 1),
(N'Stephen Hawking', N'Author of "A Brief History of Time".', 1),
(N'Carl Sagan', N'Author of "Cosmos".', 1),
(N'Richard Dawkins', N'Author of "The Selfish Gene".', 1),
(N'Daniel Kahneman', N'Author of "Thinking, Fast and Slow".', 1),
(N'Frank Herbert', N'Author of "Dune".', 1),
(N'Isaac Asimov', N'Author of "Foundation".', 1),
(N'Orson Scott Card', N'Author of "Ender''s Game".', 1),
(N'George R.R. Martin', N'Author of "A Game of Thrones".', 1),
(N'Walter Isaacson', N'Author of "Steve Jobs".', 1),
(N'Michelle Obama', N'Author of "Becoming".', 1),
(N'Anne Frank', N'Author of "The Diary of a Young Girl".', 1),
(N'Ashlee Vance', N'Author of "Elon Musk".', 1),
(N'Adam Smith', N'Author of "The Wealth of Nations".', 1);
GO

-- Thêm Sách (Book) - 50 cuốn
INSERT INTO [Book]
    ([BookName], [BookDescription], [BookPrice], [BookQuantity], [IsActive], [PublisherId], [SupplierId], [ImageURL1], [AddedAt])
VALUES
-- Programming (1-10)
(N'Clean Code', N'A book about writing clean, maintainable code.', 250000, 20, 1, 4, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1436202607i/3735293.jpg', GETDATE()),
(N'The Pragmatic Programmer', N'A classic book on software engineering.', 300000, 15, 1, 4, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1401432508i/4099.jpg', GETDATE()),
(N'Refactoring', N'Improving the design of existing code.', 280000, 10, 1, 4, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1386925632i/44936.jpg', GETDATE()),
(N'Design Patterns', N'Elements of Reusable Object-Oriented Software.', 350000, 8, 1, 4, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1348027904i/85009.jpg', GETDATE()),
(N'Introduction to Algorithms', N'The famous CLRS algorithms book.', 500000, 5, 1, 5, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1387741681i/108986.jpg', GETDATE()),
(N'The Mythical Man-Month', N'Essays on Software Engineering.', 220000, 10, 1, 4, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1348430512i/13629.jpg', GETDATE()),
(N'Code Complete 2', N'A Practical Handbook of Software Construction.', 450000, 5, 1, 5, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1396837641i/4845.jpg', GETDATE()),
(N'Head First Design Patterns', N'A brain-friendly guide to design patterns.', 380000, 12, 1, 5, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1408728856i/58128.jpg', GETDATE()),
(N'Cracking the Coding Interview', N'Prep guide for technical interviews.', 400000, 25, 1, 5, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1391084426i/12544648.jpg', GETDATE()),
(N'Structure and Interpretation of Computer Programs', N'SICP book.', 480000, 7, 1, 5, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1391032527i/43713.jpg', GETDATE()),

-- Business & Economics (11-17)
(N'Rich Dad Poor Dad', N'What the Rich Teach Their Kids About Money.', 180000, 30, 1, 1, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1388211242i/69571.jpg', GETDATE()),
(N'The Intelligent Investor', N'The Definitive Book on Value Investing.', 320000, 15, 1, 7, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1391639361i/106835.jpg', GETDATE()),
(N'Think and Grow Rich', N'A classic on personal success.', 150000, 20, 1, 1, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1463241782i/30186948.jpg', GETDATE()),
(N'Freakonomics', N'A Rogue Economist Explores the Hidden Side of Everything.', 210000, 18, 1, 7, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1550917925i/1202.jpg', GETDATE()),
(N'Zero to One', N'Notes on Startups, or How to Build the Future.', 230000, 22, 1, 6, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1414347376i/18050143.jpg', GETDATE()),
(N'The Wealth of Nations', N'The foundational work of classical economics.', 300000, 10, 1, 6, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1388350573i/25698.jpg', GETDATE()),
(N'Thinking, Fast and Slow', N'How our minds work.', 260000, 15, 1, 6, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1317793965i/11468377.jpg', GETDATE()),

-- Literature (18-26)
(N'1984', N'A dystopian social science fiction novel.', 140000, 25, 1, 6, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1657781256i/61439040.jpg', GETDATE()),
(N'The Great Gatsby', N'A novel about the American Dream.', 130000, 15, 1, 6, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1490528560i/4671.jpg', GETDATE()),
(N'To Kill a Mockingbird', N'A novel about justice and inequality.', 150000, 30, 1, 7, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1553383690i/2657.jpg', GETDATE()),
(N'Pride and Prejudice', N'A romantic novel by Jane Austen.', 120000, 20, 1, 6, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1320399351i/1885.jpg', GETDATE()),
(N'The Alchemist', N'A novel by Paulo Coelho.', 110000, 40, 1, 7, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1654371463i/18144590.jpg', GETDATE()),
(N'Norwegian Wood', N'A novel by Haruki Murakami.', 160000, 18, 1, 6, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1386924361i/11297.jpg', GETDATE()),
(N'Gone with the Wind', N'A novel by Margaret Mitchell.', 220000, 10, 1, 7, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1328025229i/18405.jpg', GETDATE()),
(N'Ticket to Childhood', N'A novel by Nguyen Nhat Anh.', 80000, 30, 1, 1, 1, N'https://salt.tikicdn.com/cache/w1200/ts/product/5e/18/24/2a6154ba08df6ce6161c13f4303fa19e.jpg', GETDATE()),
(N'The Catcher in the Rye', N'A novel by J.D. Salinger.', 130000, 12, 1, 6, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1398034300i/5107.jpg', GETDATE()),

-- Self-Help (27-32)
(N'How to Win Friends and Influence People', N'The art of building relationships.', 150000, 50, 1, 1, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1442726934i/4865.jpg', GETDATE()),
(N'The 7 Habits of Highly Effective People', N'A guide to personal effectiveness.', 240000, 25, 1, 1, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1421842784i/36072.jpg', GETDATE()),
(N'Atomic Habits', N'Tiny Changes, Remarkable Results.', 260000, 40, 1, 6, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1535115320i/40121378.jpg', GETDATE()),
(N'The Power of Habit', N'Why We Do What We Do in Life and Business.', 230000, 20, 1, 6, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1545854312i/12609433.jpg', GETDATE()),
(N'Man''s Search for Meaning', N'The classic tribute to hope.', 170000, 15, 1, 1, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1535419394i/4069.jpg', GETDATE()),
(N'Start with Why', N'How Great Leaders Inspire Everyone.', 220000, 18, 1, 6, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1360019320i/7108725.jpg', GETDATE()),

-- Children's Books (33-37)
(N'The Little Prince', N'A poetic tale by Antoine de Saint-Exupéry.', 100000, 30, 1, 2, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1367545443i/157993.jpg', GETDATE()),
(N'Charlotte''s Web', N'A beloved story of friendship.', 90000, 20, 1, 2, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1439632243i/24178.jpg', GETDATE()),
(N'Where the Wild Things Are', N'A classic children''s picture book.', 110000, 15, 1, 2, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1384818988i/19543.jpg', GETDATE()),
(N'Doraemon Vol. 1', N'A Japanese manga series.', 50000, 50, 1, 2, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1348089024i/762993.jpg', GETDATE()),
(N'The Cat in the Hat', N'A classic Dr. Seuss book.', 95000, 25, 1, 2, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1468411083i/233093.jpg', GETDATE()),

-- Science & Technology (38-42)
(N'Sapiens: A Brief History of Humankind', N'A look at the history of humanity.', 450000, 12, 1, 2, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1595674533i/23692271.jpg', GETDATE()),
(N'A Brief History of Time', N'From the Big Bang to Black Holes.', 270000, 14, 1, 3, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1333578746i/3869.jpg', GETDATE()),
(N'Cosmos', N'A journey through the universe.', 310000, 10, 1, 3, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1388359359i/55030.jpg', GETDATE()),
(N'The Selfish Gene', N'A book on evolution.', 240000, 9, 1, 3, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1366758096i/61535.jpg', GETDATE()),
(N'Homo Deus: A Brief History of Tomorrow', N'A look into the future of humanity.', 290000, 11, 1, 2, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1468760805i/31138556.jpg', GETDATE()),

-- Fantasy & Sci-Fi (43-46)
(N'Dune', N'A landmark science fiction novel.', 330000, 20, 1, 1, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1555447414i/44767458.jpg', GETDATE()),
(N'Foundation', N'A science fiction series by Isaac Asimov.', 310000, 15, 1, 1, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1417900846i/29579.jpg', GETDATE()),
(N'Ender''s Game', N'A military science fiction novel.', 200000, 18, 1, 1, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1408303130i/375802.jpg', GETDATE()),
(N'A Game of Thrones', N'The first book in A Song of Ice and Fire.', 350000, 25, 1, 7, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1562726234i/13496.jpg', GETDATE()),

-- Biography (47-50)
(N'Steve Jobs', N'The biography by Walter Isaacson.', 400000, 15, 1, 1, 1, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1511288482i/11084145.jpg', GETDATE()),
(N'Becoming', N'The memoir of Michelle Obama.', 380000, 20, 1, 6, 2, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1528206996i/38746485.jpg', GETDATE()),
(N'The Diary of a Young Girl', N'The diary of Anne Frank.', 160000, 22, 1, 2, 3, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1560816565i/48855.jpg', GETDATE()),
(N'Elon Musk', N'The biography by Ashlee Vance.', 360000, 17, 1, 1, 4, N'https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1518291452i/25541028.jpg', GETDATE());
GO

-- Thêm BookCategory (Liên kết Sách - Thể loại)
-- (CatId: 1=Prog, 2=Biz, 3=Lit, 4=Self-Help, 5=Kids, 6=SciTech, 7=Bio, 8=Fantasy)
INSERT INTO BookCategory (BookId, CategoryId, IsActive)
VALUES
(1, 1, 1), (2, 1, 1), (3, 1, 1), (4, 1, 1), (5, 1, 1),
(6, 1, 1), (7, 1, 1), (8, 1, 1), (9, 1, 1), (10, 1, 1),
(11, 2, 1), (12, 2, 1), (13, 2, 1), (14, 2, 1), (15, 2, 1),
(16, 2, 1), (17, 2, 1), (17, 4, 1), -- Thinking, Fast and Slow (Biz/Self-Help)
(18, 3, 1), (18, 8, 1), -- 1984 (Lit/Sci-Fi)
(19, 3, 1), (20, 3, 1), (21, 3, 1), (22, 3, 1), (23, 3, 1),
(24, 3, 1), (25, 3, 1), (26, 3, 1),
(27, 4, 1), (28, 4, 1), (29, 4, 1), (30, 4, 1), (31, 4, 1),
(31, 7, 1), -- Man's Search (Self-Help/Bio)
(32, 4, 1), (32, 2, 1), -- Start with Why (Self-Help/Biz)
(33, 5, 1), (33, 3, 1), -- Little Prince (Kids/Lit)
(34, 5, 1), (35, 5, 1), (36, 5, 1), (37, 5, 1),
(38, 6, 1), (39, 6, 1), (40, 6, 1), (41, 6, 1), (42, 6, 1),
(43, 8, 1), (44, 8, 1), (45, 8, 1), (46, 8, 1), (46, 3, 1), -- GoT (Fantasy/Lit)
(47, 7, 1), (48, 7, 1), (49, 7, 1), (50, 7, 1);
GO

-- Thêm BookAuthor (Liên kết Sách - Tác giả)
-- (Author IDs tương ứng với thứ tự INSERT ở trên)
INSERT INTO BookAuthor (BookId, AuthorId, IsActive)
VALUES
(1, 1, 1), (2, 2, 1), (3, 2, 1), (4, 3, 1), (5, 6, 1),
(6, 8, 1), (7, 9, 1), (8, 10, 1), (9, 11, 1), (10, 6, 1), -- Giả sử
(11, 12, 1), (12, 13, 1), (13, 14, 1), (14, 15, 1), (15, 16, 1),
(16, 49, 1), (17, 40, 1), (18, 17, 1), (19, 18, 1), (20, 19, 1),
(21, 21, 1), (22, 24, 1), (23, 25, 1), (24, 26, 1), (25, 4, 1),
(26, 20, 1), (27, 5, 1), (28, 27, 1), (29, 28, 1), (30, 29, 1),
(31, 30, 1), (32, 31, 1), (33, 32, 1), (34, 33, 1), (35, 34, 1),
(36, 35, 1), (37, 36, 1), (38, 7, 1), (39, 37, 1), (40, 38, 1),
(41, 39, 1), (42, 7, 1), (43, 41, 1), (44, 42, 1), (45, 43, 1),
(46, 44, 1), (47, 45, 1), (48, 46, 1), (49, 47, 1), (50, 48, 1);
GO

-- Thêm Voucher
INSERT INTO Voucher (VoucherCode, DiscountPercent, ExpriryDate, MinOrderAmount, MaxOrderAmount, UsageCount, IsActive, VoucherDescription)
VALUES
('NEWUSER10', 10, '2025-12-31', 100000, 500000, 100, 1, N'10% off for new customers'),
('BIGSALE15', 15, '2026-01-31', 500000, 1500000, 200, 1, N'15% off for orders from 500,000'),
('VIP20', 20, '2026-06-30', 1000000, 2000000, 50, 1, N'VIP Voucher 20% off for large orders'),
('HOLIDAY5', 5, '2025-12-25', 100000, 2000000, 500, 1, N'5% off for holiday season'),
('BLACKFRIDAY25', 25, '2025-11-30', 300000, 2000000, 300, 1, N'25% off on Black Friday'),
('EXPIRED10', 10, '2024-12-31', 100000, 2000000, 0, 0, N'Expired voucher - for testing');
GO

-- Thêm vào giỏ hàng (Cart)
INSERT INTO Cart (UserId, BookId, Quantity)
VALUES
((SELECT UserId FROM [User] WHERE Username = 'cus02'), 27, 1), -- 1x How to Win Friends...
((SELECT UserId FROM [User] WHERE Username = 'cus02'), 25, 2); -- 2x Ticket to Childhood
GO

-- ===================================================================
-- 3. TẠO DỮ LIỆU GIAO DỊCH (ORDER, PAYMENT, FEEDBACK)
-- ===================================================================

-- Khai báo biến dùng chung
DECLARE @Cus01Id INT = (SELECT UserId FROM [User] WHERE Username = 'cus01');
DECLARE @Cus01Addr NVARCHAR(255) = (SELECT Address FROM [User] WHERE Username = 'cus01');
DECLARE @OrderId_1 INT;
DECLARE @PaymentId_1 INT;

-- Tạo một đơn hàng (Order) đã hoàn thành cho 'cus01'
INSERT INTO [Order] (UserId, Address, OrderDate, deliveredDate, Status, TotalAmount, Discount, UpdateAt)
VALUES
(@Cus01Id, @Cus01Addr, '2025-11-01 10:30:00', '2025-11-03 14:00:00', N'Delivered', 675000, 75000, '2025-11-03 14:00:00');
SET @OrderId_1 = SCOPE_IDENTITY(); -- Lấy OrderId vừa tạo

-- Thêm chi tiết đơn hàng (OrderDetail) cho Order 1
INSERT INTO OrderDetail (OrderId, BookId, Quantity, Price)
VALUES
(@OrderId_1, 1, 1, 250000), -- BookId 1 (Clean Code) @ 250,000
(@OrderId_1, 5, 1, 500000); -- BookId 5 (Algorithms) @ 500,000

-- Thêm thanh toán (Payment) cho Order 1
INSERT INTO Payment (OrderId, PaymentMethod, PaymentStatus, TransactionId, Amount, CreatedAt, PaidAt)
VALUES
(@OrderId_1, 'MOMO', 'Success', 'MOMO_T12345678', 675000, '2025-11-01 10:31:00', '2025-11-01 10:31:00');
SET @PaymentId_1 = SCOPE_IDENTITY(); -- Lấy PaymentId vừa tạo

-- Cập nhật [Order].PaymentId
UPDATE [Order]
SET PaymentId = @PaymentId_1
WHERE OrderId = @OrderId_1;

-- Thêm dữ liệu sử dụng Voucher (UserVoucher) cho Order 1
INSERT INTO UserVoucher (UserId, VoucherId, OrderId, UsedAt)
VALUES
(
    @Cus01Id,
    (SELECT VoucherId FROM Voucher WHERE VoucherCode = 'BIGSALE15'),
    @OrderId_1,
    '2025-11-01 10:30:00'
);

-- Thêm Feedback cho Order 1
INSERT INTO Feedback (UserId, BookId, OrderId, CreateAt, Star, Comment, IsActive)
VALUES
(
    @Cus01Id,
    1, -- Feedback cho sách 'Clean Code'
    @OrderId_1,
    '2025-11-05 09:00:00',
    5,
    N'Great book and very helpful! Fast delivery.',
    1
);
GO