-- Thêm dữ liệu vào Role
INSERT INTO [Role] (Name) VALUES
(N'Admin'),
(N'Employee'),
(N'Customer');
GO
INSERT INTO [User] (Username, DateOfBirth, Gender, Address, Name, Email, Phone, Password)
VALUES
-- Admin
(N'admin01', '1990-05-12', N'Male',   N'123 Đường A, Hà Nội', N'Nguyễn Văn Admin',
 N'admin@book.com', '0901234567',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),

-- Employee
(N'emp01', '1995-03-22', N'Female', N'45 Đường B, TP.HCM', N'Trần Thị Nhân Viên',
 N'employee@book.com', '0912345678',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'),

-- Customer
(N'cus01', '2000-11-01', N'Male',   N'78 Đường C, Đà Nẵng', N'Lê Văn Khách',
 N'customer@book.com', '0923456789',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92');
GO

--Gán Role cho từng User
INSERT INTO [UserRole] (UserId, RoleId)
VALUES (
    (SELECT UserId FROM [User] WHERE Username = 'admin01'),
    (SELECT RoleId FROM [Role] WHERE Name = 'Admin')
);

INSERT INTO [UserRole] (UserId, RoleId)
VALUES (
    (SELECT UserId FROM [User] WHERE Username = 'emp01'),
    (SELECT RoleId FROM [Role] WHERE Name = 'Employee')
);

INSERT INTO [UserRole] (UserId, RoleId)
VALUES (
    (SELECT UserId FROM [User] WHERE Username = 'cus01'),
    (SELECT RoleId FROM [Role] WHERE Name = 'Customer')
);

INSERT INTO [WorldBookDB].[dbo].[Book]
    ([BookName], [BookDescription], [BookPrice], [BookQuantity],
     [IsActive], [PublisherId], [SupplierId],
     [ImageURL1], [ImageURL2], [ImageURL3], [ImageURL4], [AddedAt])
VALUES
-- 1
(N'Clean Code', 
 N'Sách về cách viết mã sạch, dễ hiểu và dễ bảo trì.', 
 250000, 20, 1, 1, 1,
 N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBqfcfo45TwzGiZDrzp0Dr9j0Cxom621QI9w&s', NULL, NULL, NULL, GETDATE()),

-- 2
(N'The Pragmatic Programmer', 
 N'Sách lập trình kinh điển giúp cải thiện tư duy kỹ sư phần mềm.', 
 300000, 15, 1, 2, 2,
 N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBqfcfo45TwzGiZDrzp0Dr9j0Cxom621QI9w&s', NULL, NULL, NULL, GETDATE()),

-- 3
(N'Refactoring', 
 N'Hướng dẫn tái cấu trúc mã nguồn để nâng cao chất lượng.', 
 280000, 10, 1, 1, 3,
 N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBqfcfo45TwzGiZDrzp0Dr9j0Cxom621QI9w&s', NULL, NULL, NULL, GETDATE()),

-- 4
(N'Design Patterns', 
 N'Giới thiệu các mẫu thiết kế phần mềm phổ biến.', 
 350000, 8, 1, 2, 4,
 N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBqfcfo45TwzGiZDrzp0Dr9j0Cxom621QI9w&s', NULL, NULL, NULL, GETDATE()),

-- 5
(N'Introduction to Algorithms', 
 N'Sách thuật toán nổi tiếng (CLRS).', 
 500000, 5, 1, 3, 1,
 N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBqfcfo45TwzGiZDrzp0Dr9j0Cxom621QI9w&s', NULL, NULL, NULL, GETDATE());
