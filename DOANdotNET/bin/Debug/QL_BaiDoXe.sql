-- ============================================================
--  QL_BaiDoXe - Script tạo database + dữ liệu demo
--  Bao gồm: Users, LoaiXe, ParkingSlot, Transactions, DatTruoc
--  Chạy bằng SSMS, kết nối: LAPTOP-IAOD51E6\SQLEXPRESS
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QL_BaiDoXe')
BEGIN
    ALTER DATABASE QL_BaiDoXe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QL_BaiDoXe;
END
GO

CREATE DATABASE QL_BaiDoXe;
GO

USE QL_BaiDoXe;
GO

-- ============================================================
--  BẢNG: Users
-- ============================================================
CREATE TABLE Users (
    ID          VARCHAR(50)     NOT NULL PRIMARY KEY,
    HoTen       NVARCHAR(100)   NOT NULL,
    SDT         VARCHAR(15)     NULL,
    BienSo      VARCHAR(20)     NULL,
    VaiTro      VARCHAR(20)     NOT NULL,   -- 'admin' | 'nhanvien' | 'khachhang'
    MatKhau     VARCHAR(255)    NULL,
    NgayTao     DATETIME        NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
--  BẢNG: LoaiXe
-- ============================================================
CREATE TABLE LoaiXe (
    MaLoaiXe    VARCHAR(20)     NOT NULL PRIMARY KEY,
    TenLoaiXe   NVARCHAR(50)    NOT NULL,
    PhiMoiGio   DECIMAL(18,0)   NOT NULL
);
GO

-- ============================================================
--  BẢNG: ParkingSlot
-- ============================================================
CREATE TABLE ParkingSlot (
    IDDoXe          VARCHAR(20)     NOT NULL PRIMARY KEY,
    KhuVuc          NVARCHAR(50)    NOT NULL,
    IDNguoiDung     VARCHAR(50)     NULL,
    TenKhachHang    NVARCHAR(100)   NULL,
    SoDienThoai     VARCHAR(15)     NULL,
    BienSoXe        VARCHAR(20)     NULL,
    ThoiGianVao     DATETIME        NULL,
    MaLoaiXe        VARCHAR(20)     NULL,
    HinhAnhVao      NVARCHAR(MAX)   NULL,
    TrangThai       NVARCHAR(20)    NOT NULL DEFAULT N'Trống',

    CONSTRAINT FK_ParkingSlot_Users  FOREIGN KEY (IDNguoiDung) REFERENCES Users(ID),
    CONSTRAINT FK_ParkingSlot_LoaiXe FOREIGN KEY (MaLoaiXe)    REFERENCES LoaiXe(MaLoaiXe)
);
GO

-- ============================================================
--  BẢNG: Transactions
-- ============================================================
CREATE TABLE Transactions (
    MaGiaoDich      VARCHAR(50)     NOT NULL PRIMARY KEY,
    IDNguoiDung     VARCHAR(50)     NULL,
    IDDoXe          VARCHAR(20)     NULL,
    TenKhachHang    NVARCHAR(100)   NULL,
    SoDienThoai     VARCHAR(15)     NULL,
    BienSoXe        VARCHAR(20)     NOT NULL,
    MaLoaiXe        VARCHAR(20)     NOT NULL,
    ThoiGianVao     DATETIME        NOT NULL,
    ThoiGianRa      DATETIME        NULL,
    ThanhTien       DECIMAL(18,0)   NOT NULL DEFAULT 0,
    HinhAnhVao      NVARCHAR(MAX)   NULL,
    HinhAnhRa       NVARCHAR(MAX)   NULL,

    CONSTRAINT FK_Transactions_Users  FOREIGN KEY (IDNguoiDung) REFERENCES Users(ID),
    CONSTRAINT FK_Transactions_LoaiXe FOREIGN KEY (MaLoaiXe)    REFERENCES LoaiXe(MaLoaiXe)
);
GO

-- ============================================================
--  BẢNG: DatTruoc
-- ============================================================
CREATE TABLE DatTruoc (
    MaDatTruoc      VARCHAR(50)     NOT NULL PRIMARY KEY,
    IDKhachHang     VARCHAR(50)     NOT NULL,
    KhuVuc          NVARCHAR(50)    NOT NULL,
    MaLoaiXe        VARCHAR(20)     NOT NULL,
    ThoiGianDen     DATETIME        NOT NULL,
    ThoiGianHetHan  DATETIME        NOT NULL,   -- = ThoiGianDen + 30 phut
    TrangThai       NVARCHAR(20)    NOT NULL DEFAULT N'Chờ xử lý',
                    -- 'Chờ xử lý' | 'Đã xác nhận' | 'Đã hủy' | 'Hoàn thành'
    IDDoXe          VARCHAR(20)     NULL,        -- slot được xếp sau khi xác nhận
    GhiChu          NVARCHAR(200)   NULL,
    NgayTao         DATETIME        NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_DatTruoc_Users  FOREIGN KEY (IDKhachHang) REFERENCES Users(ID),
    CONSTRAINT FK_DatTruoc_LoaiXe FOREIGN KEY (MaLoaiXe)    REFERENCES LoaiXe(MaLoaiXe)
);
GO

-- ============================================================
--  DỮ LIỆU: Users
-- ============================================================
INSERT INTO Users (ID, HoTen, SDT, BienSo, VaiTro, MatKhau, NgayTao) VALUES
('admin', N'Nguyễn Văn Admin', '0901000001', '51A-00001', 'admin',      'admin123', '2025-01-01'),
('nv01',  N'Trần Thị Lan',     '0901000002', '51B-11111', 'nhanvien',   'nv456',    '2025-02-01'),
('nv02',  N'Lê Văn Minh',      '0901000003', '51C-22222', 'nhanvien',   'nv789',    '2025-03-01'),
('kh01',  N'Phạm Thị Thu',     '0901000004', '51D-33333', 'khachhang',  'kh123',    '2025-04-01'),
('kh02',  N'Hoàng Văn Bình',   '0901000005', '51E-44444', 'khachhang',  'kh456',    '2025-04-15');
GO

-- ============================================================
--  DỮ LIỆU: LoaiXe
-- ============================================================
INSERT INTO LoaiXe (MaLoaiXe, TenLoaiXe, PhiMoiGio) VALUES
('LX001', N'Xe máy',     5000),
('LX002', N'Ô tô',       20000),
('LX003', N'Xe đạp',     2000),
('LX004', N'Xe tải nhỏ', 30000);
GO

-- ============================================================
--  DỮ LIỆU: ParkingSlot (20 slot, 5 slot đang có xe)
-- ============================================================
INSERT INTO ParkingSlot (IDDoXe, KhuVuc, TrangThai) VALUES
('A01', N'Khu A', N'Trống'), ('A02', N'Khu A', N'Trống'),
('A03', N'Khu A', N'Trống'), ('A04', N'Khu A', N'Trống'),
('A05', N'Khu A', N'Trống'), ('A06', N'Khu A', N'Trống'),
('A07', N'Khu A', N'Trống'), ('A08', N'Khu A', N'Trống'),
('A09', N'Khu A', N'Trống'), ('A10', N'Khu A', N'Trống'),
('B01', N'Khu B', N'Trống'), ('B02', N'Khu B', N'Trống'),
('B03', N'Khu B', N'Trống'), ('B04', N'Khu B', N'Trống'),
('B05', N'Khu B', N'Trống'), ('B06', N'Khu B', N'Trống'),
('C01', N'Khu C', N'Trống'), ('C02', N'Khu C', N'Trống'),
('C03', N'Khu C', N'Trống'), ('C04', N'Khu C', N'Trống');
GO

UPDATE ParkingSlot SET TrangThai=N'Đang gửi', IDNguoiDung='nv01',
    TenKhachHang=N'Phạm Văn An',      SoDienThoai='0912345678',
    BienSoXe='51A-123.45', MaLoaiXe='LX001',
    ThoiGianVao=DATEADD(HOUR,-2,GETDATE()) WHERE IDDoXe='A01';

UPDATE ParkingSlot SET TrangThai=N'Đang gửi', IDNguoiDung='nv01',
    TenKhachHang=N'Nguyễn Thị Bích',  SoDienThoai='0923456789',
    BienSoXe='51B-678.90', MaLoaiXe='LX001',
    ThoiGianVao=DATEADD(MINUTE,-45,GETDATE()) WHERE IDDoXe='A03';

UPDATE ParkingSlot SET TrangThai=N'Đang gửi', IDNguoiDung='admin',
    TenKhachHang=N'Lê Quốc Cường',    SoDienThoai='0934567890',
    BienSoXe='51C-999.11', MaLoaiXe='LX002',
    ThoiGianVao=DATEADD(HOUR,-3,GETDATE()) WHERE IDDoXe='B01';

UPDATE ParkingSlot SET TrangThai=N'Đang gửi', IDNguoiDung='nv02',
    TenKhachHang=N'Trần Minh Đức',    SoDienThoai='0945678901',
    BienSoXe='51D-555.22', MaLoaiXe='LX002',
    ThoiGianVao=DATEADD(MINUTE,-90,GETDATE()) WHERE IDDoXe='B03';

UPDATE ParkingSlot SET TrangThai=N'Đang gửi', IDNguoiDung='nv02',
    TenKhachHang=N'Võ Thị Hoa',       SoDienThoai='0956789012',
    BienSoXe='51E-333.44', MaLoaiXe='LX003',
    ThoiGianVao=DATEADD(MINUTE,-20,GETDATE()) WHERE IDDoXe='C01';
GO

-- ============================================================
--  DỮ LIỆU: Transactions
-- ============================================================

-- Hôm nay: hoàn thành
INSERT INTO Transactions (MaGiaoDich,IDNguoiDung,IDDoXe,TenKhachHang,SoDienThoai,BienSoXe,MaLoaiXe,ThoiGianVao,ThoiGianRa,ThanhTien) VALUES
('GD-TODAY-001','nv01','A02',N'Hoàng Văn Em',   '0911111111','51A-001.11','LX001',DATEADD(HOUR,-5,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),DATEADD(HOUR,-2,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),15000),
('GD-TODAY-002','nv01','A04',N'Đinh Thị Phương','0922222222','51A-002.22','LX001',DATEADD(HOUR,-4,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),DATEADD(HOUR,-1,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),10000),
('GD-TODAY-003','admin','B02',N'Phan Văn Giang', '0933333333','51B-003.33','LX002',DATEADD(HOUR,-6,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),DATEADD(HOUR,-3,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),60000),
('GD-TODAY-004','nv02','C02',N'Bùi Thị Hạnh',   '0944444444','51C-004.44','LX003',DATEADD(HOUR,-3,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),DATEADD(HOUR,-1,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),4000),
('GD-TODAY-005','nv01','A06',N'Lý Văn Khánh',   '0955555555','51A-005.55','LX001',DATEADD(HOUR,-2,CAST(CAST(GETDATE()AS DATE)AS DATETIME)),DATEADD(MINUTE,-30,GETDATE()),10000);

-- Đang gửi (khớp ParkingSlot)
INSERT INTO Transactions (MaGiaoDich,IDNguoiDung,IDDoXe,TenKhachHang,SoDienThoai,BienSoXe,MaLoaiXe,ThoiGianVao,ThoiGianRa,ThanhTien) VALUES
('GD-ACTIVE-001','nv01', 'A01',N'Phạm Văn An',    '0912345678','51A-123.45','LX001',DATEADD(HOUR,-2,GETDATE()),   NULL,0),
('GD-ACTIVE-002','nv01', 'A03',N'Nguyễn Thị Bích','0923456789','51B-678.90','LX001',DATEADD(MINUTE,-45,GETDATE()),NULL,0),
('GD-ACTIVE-003','admin','B01',N'Lê Quốc Cường',  '0934567890','51C-999.11','LX002',DATEADD(HOUR,-3,GETDATE()),   NULL,0),
('GD-ACTIVE-004','nv02', 'B03',N'Trần Minh Đức',  '0945678901','51D-555.22','LX002',DATEADD(MINUTE,-90,GETDATE()),NULL,0),
('GD-ACTIVE-005','nv02', 'C01',N'Võ Thị Hoa',     '0956789012','51E-333.44','LX003',DATEADD(MINUTE,-20,GETDATE()),NULL,0);

-- Lịch sử 7 ngày
INSERT INTO Transactions (MaGiaoDich,IDNguoiDung,IDDoXe,TenKhachHang,SoDienThoai,BienSoXe,MaLoaiXe,ThoiGianVao,ThoiGianRa,ThanhTien) VALUES
('GD-D1-001','nv01', 'A01',N'KH Ngẫu Nhiên 1', '0900000001','51A-101.01','LX001',DATEADD(DAY,-1,GETDATE()),DATEADD(HOUR, 2,DATEADD(DAY,-1,GETDATE())), 10000),
('GD-D1-002','nv02', 'B01',N'KH Ngẫu Nhiên 2', '0900000002','51B-202.02','LX002',DATEADD(DAY,-1,GETDATE()),DATEADD(HOUR, 3,DATEADD(DAY,-1,GETDATE())), 60000),
('GD-D1-003','nv01', 'A02',N'KH Ngẫu Nhiên 3', '0900000003','51A-303.03','LX001',DATEADD(DAY,-1,GETDATE()),DATEADD(HOUR, 1,DATEADD(DAY,-1,GETDATE())),  5000),
('GD-D2-001','admin','B02',N'KH Ngẫu Nhiên 4', '0900000004','51C-404.04','LX002',DATEADD(DAY,-2,GETDATE()),DATEADD(HOUR, 4,DATEADD(DAY,-2,GETDATE())), 80000),
('GD-D2-002','nv01', 'A03',N'KH Ngẫu Nhiên 5', '0900000005','51A-505.05','LX001',DATEADD(DAY,-2,GETDATE()),DATEADD(HOUR, 2,DATEADD(DAY,-2,GETDATE())), 10000),
('GD-D2-003','nv02', 'C01',N'KH Ngẫu Nhiên 6', '0900000006','51C-606.06','LX003',DATEADD(DAY,-2,GETDATE()),DATEADD(HOUR, 1,DATEADD(DAY,-2,GETDATE())),  2000),
('GD-D3-001','nv01', 'A04',N'KH Ngẫu Nhiên 7', '0900000007','51A-707.07','LX001',DATEADD(DAY,-3,GETDATE()),DATEADD(HOUR, 3,DATEADD(DAY,-3,GETDATE())), 15000),
('GD-D3-002','admin','B03',N'KH Ngẫu Nhiên 8', '0900000008','51D-808.08','LX004',DATEADD(DAY,-3,GETDATE()),DATEADD(HOUR, 5,DATEADD(DAY,-3,GETDATE())),150000),
('GD-D4-001','nv02', 'A05',N'KH Ngẫu Nhiên 9', '0900000009','51A-909.09','LX001',DATEADD(DAY,-4,GETDATE()),DATEADD(HOUR, 2,DATEADD(DAY,-4,GETDATE())), 10000),
('GD-D4-002','nv01', 'B04',N'KH Ngẫu Nhiên 10','0900000010','51B-010.10','LX002',DATEADD(DAY,-4,GETDATE()),DATEADD(HOUR, 3,DATEADD(DAY,-4,GETDATE())), 60000),
('GD-D5-001','admin','A06',N'KH Ngẫu Nhiên 11','0900000011','51A-011.11','LX001',DATEADD(DAY,-5,GETDATE()),DATEADD(HOUR, 1,DATEADD(DAY,-5,GETDATE())),  5000),
('GD-D5-002','nv02', 'B05',N'KH Ngẫu Nhiên 12','0900000012','51B-012.12','LX002',DATEADD(DAY,-5,GETDATE()),DATEADD(HOUR, 4,DATEADD(DAY,-5,GETDATE())), 80000),
('GD-D6-001','nv01', 'A07',N'KH Ngẫu Nhiên 13','0900000013','51A-013.13','LX001',DATEADD(DAY,-6,GETDATE()),DATEADD(HOUR, 2,DATEADD(DAY,-6,GETDATE())), 10000),
('GD-D6-002','admin','B06',N'KH Ngẫu Nhiên 14','0900000014','51B-014.14','LX002',DATEADD(DAY,-6,GETDATE()),DATEADD(HOUR, 3,DATEADD(DAY,-6,GETDATE())), 60000),
('GD-D7-001','nv01', 'A08',N'KH Ngẫu Nhiên 15','0900000015','51A-015.15','LX001',DATEADD(DAY,-7,GETDATE()),DATEADD(HOUR, 5,DATEADD(DAY,-7,GETDATE())), 25000),
('GD-D7-002','nv02', 'C02',N'KH Ngẫu Nhiên 16','0900000016','51C-016.16','LX003',DATEADD(DAY,-7,GETDATE()),DATEADD(HOUR, 1,DATEADD(DAY,-7,GETDATE())),  2000);
GO

-- ============================================================
--  DỮ LIỆU: DatTruoc (demo lịch sử đặt cho kh01, kh02)
-- ============================================================
INSERT INTO DatTruoc (MaDatTruoc,IDKhachHang,KhuVuc,MaLoaiXe,ThoiGianDen,ThoiGianHetHan,TrangThai,IDDoXe,NgayTao) VALUES
-- Đang chờ đến (sẽ đến sau 2 giờ)
('DT-001','kh01',N'Khu A','LX001',
    DATEADD(HOUR, 2,GETDATE()), DATEADD(MINUTE,150,GETDATE()),
    N'Đã xác nhận','A05', DATEADD(HOUR,-1,GETDATE())),
-- Đã hủy
('DT-002','kh01',N'Khu B','LX002',
    DATEADD(DAY,-1,GETDATE()), DATEADD(MINUTE,30,DATEADD(DAY,-1,GETDATE())),
    N'Đã hủy', NULL, DATEADD(DAY,-2,GETDATE())),
-- Hoàn thành
('DT-003','kh02',N'Khu A','LX001',
    DATEADD(DAY,-3,GETDATE()), DATEADD(MINUTE,30,DATEADD(DAY,-3,GETDATE())),
    N'Hoàn thành','A02', DATEADD(DAY,-4,GETDATE())),
-- Mới tạo, chưa xử lý
('DT-004','kh02',N'Khu C','LX003',
    DATEADD(HOUR, 5,GETDATE()), DATEADD(MINUTE,330,GETDATE()),
    N'Chờ xử lý', NULL, GETDATE());
GO

-- ============================================================
--  KIỂM TRA NHANH
-- ============================================================
SELECT 'Users'        AS [Table], COUNT(*) AS [Rows] FROM Users        UNION ALL
SELECT 'LoaiXe',       COUNT(*) FROM LoaiXe                            UNION ALL
SELECT 'ParkingSlot',  COUNT(*) FROM ParkingSlot                       UNION ALL
SELECT 'Transactions', COUNT(*) FROM Transactions                      UNION ALL
SELECT 'DatTruoc',     COUNT(*) FROM DatTruoc;

SELECT IDDoXe, TrangThai, TenKhachHang, BienSoXe FROM ParkingSlot ORDER BY IDDoXe;

SELECT ID, VaiTro, MatKhau FROM Users;
GO

PRINT '=== Hoàn tất! Tài khoản: admin/admin123 | nv01/nv456 | nv02/nv789 | kh01/kh123 | kh02/kh456 ===';
GO