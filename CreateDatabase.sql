-- =============================================
-- QUẢN LÝ KHÁCH SẠN - DATABASE SCRIPT
-- SQL Server 2019
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyKhachSan')
BEGIN
    ALTER DATABASE QuanLyKhachSan SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyKhachSan;
END
GO

CREATE DATABASE QuanLyKhachSan2;
GO

USE QuanLyKhachSan2;
GO

-- =============================================
-- BẢNG TÀI KHOẢN
-- =============================================
CREATE TABLE Account (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserName    NVARCHAR(50)  NOT NULL UNIQUE,
    DisplayName NVARCHAR(100) NOT NULL,
    PassWord    NVARCHAR(255) NOT NULL,
    Role        NVARCHAR(20)  NOT NULL DEFAULT 'staff'  -- 'admin' hoặc 'staff'
);

-- =============================================
-- BẢNG LOẠI PHÒNG
-- =============================================
CREATE TABLE RoomCategory (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    Price   DECIMAL(18,0) NOT NULL DEFAULT 0
);

-- =============================================
-- BẢNG PHÒNG
-- =============================================
CREATE TABLE Room (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber  NVARCHAR(20)  NOT NULL UNIQUE,
    Floor       INT           NOT NULL DEFAULT 1,
    IdCategory  INT           NOT NULL,
    Status      NVARCHAR(20)  NOT NULL DEFAULT N'Trống',  -- N'Trống' / N'Có người'
    FOREIGN KEY (IdCategory) REFERENCES RoomCategory(Id)
);

-- =============================================
-- BẢNG KHÁCH HÀNG
-- =============================================
CREATE TABLE Customer (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(100) NOT NULL,
    IdCard      NVARCHAR(20)  NOT NULL,
    Phone       NVARCHAR(20)  NOT NULL,
    Address     NVARCHAR(255) NULL,
    Email       NVARCHAR(100) NULL
);

-- =============================================
-- BẢNG ĐẶT PHÒNG
-- =============================================
CREATE TABLE Booking (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    CheckIn         DATETIME      NOT NULL DEFAULT GETDATE(),
    CheckOut        DATETIME      NOT NULL,
    IdRoom          INT           NOT NULL,
    IdCustomer      INT           NOT NULL,
    Status          NVARCHAR(20)  NOT NULL DEFAULT N'Đang ở',  -- N'Đang ở' / N'Đã trả'
    TotalPrice      DECIMAL(18,0) NOT NULL DEFAULT 0,
    RoomAmount      DECIMAL(18,0) NOT NULL DEFAULT 0,
    ServiceAmount   DECIMAL(18,0) NOT NULL DEFAULT 0,
    Note            NVARCHAR(500) NULL,
    FOREIGN KEY (IdRoom)      REFERENCES Room(Id),
    FOREIGN KEY (IdCustomer)  REFERENCES Customer(Id)
);

-- =============================================
-- BẢNG DỊCH VỤ
-- =============================================
CREATE TABLE Service (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(100) NOT NULL,
    Price       DECIMAL(18,0) NOT NULL DEFAULT 0,
    IsActive    BIT           NOT NULL DEFAULT 1
);

-- =============================================
-- BẢNG DỊCH VỤ TRONG BOOKING
-- =============================================
CREATE TABLE BookingService (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    IdBooking   INT NOT NULL,
    IdService   INT NOT NULL,
    Count       INT NOT NULL DEFAULT 1,
    UnitPrice   DECIMAL(18,0) NOT NULL DEFAULT 0,
    AddedAt     DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IdBooking) REFERENCES Booking(Id),
    FOREIGN KEY (IdService) REFERENCES Service(Id)
);

-- =============================================
-- DỮ LIỆU MẪU
-- =============================================

-- Tài khoản
INSERT INTO Account (UserName, DisplayName, PassWord, Role) VALUES
(N'admin', N'Quản trị viên', N'admin123', N'admin'),
(N'thao1', N'Nhân viên 1',  N'thao1208', N'staff'),
(N'lanh', N'Nhân viên 2',  N'lananh', N'staff');

-- Loại phòng
INSERT INTO RoomCategory (Name, Price) VALUES
(N'Standard',  500000),
(N'Deluxe',    800000),
(N'Suite',    1500000),
(N'Family',   1200000);

-- Phòng Tầng 1
INSERT INTO Room (RoomNumber, Floor, IdCategory, Status) VALUES
(N'101', 1, 1, N'Trống'),
(N'102', 1, 1, N'Trống'),
(N'103', 1, 2, N'Trống'),
(N'104', 1, 2, N'Trống'),
(N'105', 1, 3, N'Trống');

-- Phòng Tầng 2
INSERT INTO Room (RoomNumber, Floor, IdCategory, Status) VALUES
(N'201', 2, 1, N'Trống'),
(N'202', 2, 1, N'Trống'),
(N'203', 2, 2, N'Trống'),
(N'204', 2, 3, N'Trống'),
(N'205', 2, 4, N'Trống');

-- Phòng Tầng 3
INSERT INTO Room (RoomNumber, Floor, IdCategory, Status) VALUES
(N'301', 3, 2, N'Trống'),
(N'302', 3, 2, N'Trống'),
(N'303', 3, 3, N'Trống'),
(N'304', 3, 4, N'Trống'),
(N'305', 3, 4, N'Trống');

-- Dịch vụ
INSERT INTO Service (ServiceName, Price) VALUES
(N'Giặt ủi',           50000),
(N'Ăn sáng',           80000),
(N'Đưa đón sân bay',  200000),
(N'Spa & Massage',    300000),
(N'Thuê xe máy',      150000),
(N'Minibar',          100000),
(N'Phòng gym',         50000),
(N'Bể bơi',            30000);

-- Khách hàng mẫu
INSERT INTO Customer (FullName, IdCard, Phone, Address, Email) VALUES
(N'Nguyễn Việt Thảo',   N'0312xxxxx34', N'0794105811', N'205, Thượng Lý, Hồng Bàng, Hải Phòng', N'nguyenvietthao1208@gmail.com'),
(N'Ngô Vy Thương',   N'0312xxxxx01', N'0912345678', N'TP.HCM', N'thuongnv@gmail.com'),
(N'Nguyễn Văn Tùng',    N'0312xxxxx02', N'0923456789', N'Đà Nẵng', N'tungnv@gmail.com');

PRINT N'Database QuanLyKhachSan tạo thành công!';
GO

----------------------------------------------------------------------
ALTER TABLE Booking ALTER COLUMN IdCustomer INT NULL;

ALTER TABLE Booking ADD CustomerNameBackup  NVARCHAR(100) NULL;
ALTER TABLE Booking ADD CustomerPhoneBackup NVARCHAR(20)  NULL;
ALTER TABLE Booking ADD CustomerIdCardBackup NVARCHAR(20) NULL;