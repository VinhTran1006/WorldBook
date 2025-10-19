CREATE database WorldBook

go 
use WorldBook

go 

create table Role(
RoleID int primary key identity(1,1),
Role varchar(50),
IsActive bit,
)

go 


create table Account(
AccountID int primary key identity(1,1),
UserName varchar(255),
UserEmail varchar(255),
PassWordhash varchar(255),
Address varchar(255),
RoleID int,
IsActive bit,
AddedAt DateTime,
foreign key (RoleID) references Role(RoleID),
)

go 

create table Category(
CategoryID int primary key identity(1,1),
CategoryName varchar(50),
CategoryDescription varchar(255),
IsActive bit
)

go

create table Publisher(
PublisherID int primary key identity(1,1),
PublisherName varchar(255),
IsActive bit,
)
go

create table Supplier(
SupplierID int primary key identity(1,1),
SupplierName varchar(255),
SupplierEmail varchar(255),
PhoneNumber varchar(10),
IsActive bit,
ContactPerson varchar(255),
Address varchar(255),
)
go

create table Author(
AuthorID int primary key identity(1,1),
AuthorName varchar(255),
AuthorDescription varchar(2550),
IsActive bit,
)

go

create table Book(
BookID int primary key identity(1,1),
BookName varchar(255),
BookDescription varchar(255),
BookPrice decimal(10,2),
BookQuantity int,
IsActive bit,
PublisherID int,
SupplierID int,
ImageURL1 varchar(255),
ImageURL2 varchar(255),
ImageURL3 varchar(255),
ImageURL4 varchar(255),
AddedAt DateTime,
foreign key (PublisherID) references Publisher(PublisherID),
foreign key (SupplierID) references Supplier(SupplierID),
)

go

create table BookCategory(
BookCategoryID int primary key identity(1,1),
BookID int,
CategoryID int,
foreign key (BookID) references Book(BookID),
foreign key (CategoryID) references Category(CategoryID),
IsActive bit,
)

go

create table BookAuthor(
BookAuthorID int primary key identity(1,1),
BookID int,
AuthorID int,
IsActive bit,
foreign key (BookID) references Book(BookID),
foreign key (AuthorID) references Author(AuthorID),
)

go 

create table Cart(
CartID int primary key identity(1,1),
AccountID int,
BookID int ,
Quantity int,
foreign key (BookID) references Book(BookID),
foreign key (AccountID) references Account(AccountID),
)

go

create table Voucher(
VoucherID int primary key identity(1,1),
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
OrderID int primary key identity(1,1),
AccountID int,
StaffID int,
Address varchar(255),
OrderDate datetime,
deliveredDate datetime,
Status varchar(50),
TotalAmount Bigint,
Discount int,
UpdateAt datetime,
foreign key (AccountID) references Account(AccountID),
foreign key (StaffID) references Account(AccountID),
)

go 

create table OrderDetail(
OrderDetailID int primary key identity(1,1),
Quantity int,
Price Bigint,
OrderID int,
BookID int,
foreign key (OrderID) references [Order](OrderID),
foreign key (BookID) references Book(BookID),
)

go

create table Feedback(
FeedbackID int primary key identity(1,1),
AccountID int, 
BookID int,
OrderID int,
CreateAt datetime,
Star int,
Comment varchar(255),
IsActive bit,
Reply varchar(255),
ReplyAccountID int,
ReplyDate datetime,
foreign key (BookID) references Book(BookID),
foreign key (AccountID) references Account(AccountID),
foreign key (OrderID) references [Order](OrderID),
foreign key (ReplyAccountID) references Account(AccountID),
)

go 

create table ImportStock(
ImportID int primary key identity(1,1),
SupplierID int,
StaffID int,
ImportDate datetime,
TotalAmount bigint,
foreign key (SupplierID) references Supplier(SupplierID),
foreign key (StaffID) references Account(AccountID),
)
go
create table ImportStockDetail(
ImportStockDetailID int primary key identity(1,1),
ImportID int,
BookID int,
Stock int,
UnitPrice bigint,
StockLeft int,
foreign key (ImportID) references ImportStock(ImportID),
foreign key (BookID) references Book(BookID),
)