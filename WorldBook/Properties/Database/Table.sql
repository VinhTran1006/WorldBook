-- Tạo database
CREATE DATABASE WorldBookDB;
GO
USE WorldBookDB;
GO


-- 3️⃣ Tạo bảng
CREATE TABLE [Role](
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE [User](
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(10) NULL,
    Address NVARCHAR(255) NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    Password NVARCHAR(255) NOT NULL,      -- Mật khẩu HASH
    IsActive BIT NOT NULL DEFAULT 1,      -- Thêm default
    AddedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE [UserRole](
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    CreateAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY(UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES [Role](RoleId) ON DELETE CASCADE
);
GO

create table Category(
CategoryId int primary key Identity(1,1),
CategoryName varchar(50),
CategoryDescription varchar(255),
IsActive bit
)

go

create table Publisher(
PublisherId int primary key Identity(1,1),
PublisherName varchar(255),
IsActive bit,
)
go

create table Supplier(
SupplierId int primary key Identity(1,1),
SupplierName varchar(255),
SupplierEmail varchar(255),
PhoneNumber varchar(10),
IsActive bit,
ContactPerson varchar(255),
Address varchar(255),
)
go

create table Author(
AuthorId int primary key Identity(1,1),
AuthorName varchar(255),
AuthorDescription varchar(2550),
IsActive bit,
)

go

create table Book(
BookId int primary key Identity(1,1),
BookName varchar(255),
BookDescription varchar(255),
BookPrice decimal(10,2),
BookQuantity int,
IsActive bit,
PublisherId int,
SupplierId int,
ImageURL1 varchar(255),
ImageURL2 varchar(255),
ImageURL3 varchar(255),
ImageURL4 varchar(255),
AddedAt DateTime,
foreign key (PublisherId) references Publisher(PublisherId),
foreign key (SupplierId) references Supplier(SupplierId),
)

go

create table BookCategory(
BookCategoryId int primary key Identity(1,1),
BookId int,
CategoryId int,
foreign key (BookId) references Book(BookId),
foreign key (CategoryId) references Category(CategoryId),
IsActive bit,
)

go

create table BookAuthor(
BookAuthorId int primary key Identity(1,1),
BookId int,
AuthorId int,
IsActive bit,
foreign key (BookId) references Book(BookId),
foreign key (AuthorId) references Author(AuthorId),
)

go 

create table Cart(
CartId int primary key Identity(1,1),
UserId int,
BookId int ,
Quantity int,
foreign key (BookId) references Book(BookId),
foreign key (UserId) references [User](UserId),
)

go

create table Voucher(
VoucherId int primary key Identity(1,1),
VoucherCode varchar(50),
DiscountPercent int,
ExpriryDate datetime,
MinOrderAmount decimal(10,2),
MaxOrderAmount decimal(10,2),
UsageCount int,
IsActive bit,
VoucherDescription varchar(255),
)

go 

create table [Order] (
OrderId int primary key Identity(1,1),
UserId int,
Address varchar(255),
OrderDate datetime,
deliveredDate datetime,
Status varchar(50),
TotalAmount Bigint,
Discount int,
UpdateAt datetime,
foreign key (UserId) references [User](UserId),
)

go 

create table OrderDetail(
OrderDetailId int primary key Identity(1,1),
Quantity int,
Price Bigint,
OrderId int,
BookId int,
foreign key (OrderId) references [Order](OrderId),
foreign key (BookId) references Book(BookId),
)

go

create table Feedback(
FeedbackId int primary key Identity(1,1),
UserId int, 
BookId int,
OrderId int,
CreateAt datetime,
Star int,
Comment varchar(255),
IsActive bit,
Reply varchar(255),
ReplyAccountId int,
ReplyDate datetime,
foreign key (BookId) references Book(BookId),
foreign key (UserId) references [User](UserId),
foreign key (OrderId) references [Order](OrderId),
foreign key (ReplyAccountId) references [User](UserId),
)

go 

create table ImportStock(
ImportId int primary key Identity(1,1),
SupplierId int,
UserId int,
ImportDate datetime,
TotalAmount bigint,
foreign key (SupplierId) references Supplier(SupplierId),
foreign key (UserId) references [User](UserId),
)
go
create table ImportStockDetail(
ImportStockDetailId int primary key Identity(1,1),
ImportId int,
BookId int,
Stock int,
UnitPrice bigint,
StockLeft int,
foreign key (ImportId) references ImportStock(ImportId),
foreign key (BookId) references Book(BookId),
)