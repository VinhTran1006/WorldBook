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