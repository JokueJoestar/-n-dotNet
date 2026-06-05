-- ============================================================
--   HỆ THỐNG QUẢN LÝ BÃI ĐỖ XE - PPLQ
--   File SQL Demo hoàn chỉnh
--   Bao gồm: Cấu trúc DB + Dữ liệu demo đầy đủ
--   Tác giả: Trần Nhật Phước
-- ============================================================

USE master;
GO

-- ============================================================
-- BƯỚC 1: TẠO DATABASE
-- ============================================================
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
-- BƯỚC 2: TẠO BẢNG
-- ============================================================

-- Bảng Loại Xe
CREATE TABLE LoaiXe (
    MaLoaiXe  VARCHAR(20)    NOT NULL PRIMARY KEY,
    TenLoaiXe NVARCHAR(50)   NOT NULL,
    PhiMoiGio DECIMAL(18, 0) NOT NULL DEFAULT 0
);
GO

-- Bảng Users (admin / nhanvien / khachhang)
CREATE TABLE Users (
    ID      VARCHAR(50)   NOT NULL PRIMARY KEY,
    HoTen   NVARCHAR(100) NOT NULL,
    SDT     VARCHAR(15)   NULL,
    BienSo  VARCHAR(20)   NULL,
    VaiTro  VARCHAR(20)   NOT NULL,   -- 'admin' | 'nhanvien' | 'khachhang'
    MatKhau VARCHAR(255)  NULL,
    NgayTao DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Vị trí đỗ xe
CREATE TABLE ParkingSlot (
    IDDoXe       VARCHAR(20)    NOT NULL PRIMARY KEY,
    KhuVuc       NVARCHAR(50)   NOT NULL,
    IDNguoiDung  VARCHAR(50)    NULL FOREIGN KEY REFERENCES Users(ID),
    TenKhachHang NVARCHAR(100)  NULL,
    SoDienThoai  VARCHAR(15)    NULL,
    BienSoXe     VARCHAR(20)    NULL,
    ThoiGianVao  DATETIME       NULL,
    MaLoaiXe     VARCHAR(20)    NULL FOREIGN KEY REFERENCES LoaiXe(MaLoaiXe),
    HinhAnhVao   NVARCHAR(MAX)  NULL,
    TrangThai    NVARCHAR(20)   NOT NULL DEFAULT N'Trống'
);
GO

-- Bảng Giao dịch
CREATE TABLE Transactions (
    MaGiaoDich   VARCHAR(50)    NOT NULL PRIMARY KEY,
    IDNguoiDung  VARCHAR(50)    NULL FOREIGN KEY REFERENCES Users(ID),
    IDDoXe       VARCHAR(20)    NULL,
    TenKhachHang NVARCHAR(100)  NULL,
    SoDienThoai  VARCHAR(15)    NULL,
    BienSoXe     VARCHAR(20)    NOT NULL,
    MaLoaiXe     VARCHAR(20)    NOT NULL FOREIGN KEY REFERENCES LoaiXe(MaLoaiXe),
    ThoiGianVao  DATETIME       NOT NULL,
    ThoiGianRa   DATETIME       NULL,
    ThanhTien    DECIMAL(18, 0) NOT NULL DEFAULT 0,
    HinhAnhVao   NVARCHAR(MAX)  NULL,
    HinhAnhRa    NVARCHAR(MAX)  NULL
);
GO

-- ============================================================
-- BƯỚC 3: LOẠI XE (5 loại)
-- ============================================================
INSERT INTO LoaiXe (MaLoaiXe, TenLoaiXe, PhiMoiGio) VALUES
('LX001', N'Xe máy',        5000),
('LX002', N'Xe ô tô 4 chỗ', 15000),
('LX003', N'Xe ô tô 7 chỗ', 20000),
('LX004', N'Xe tải nhỏ',    30000),
('LX005', N'Xe đạp',        2000);
GO

-- ============================================================
-- BƯỚC 4: TÀI KHOẢN NGƯỜI DÙNG
-- ============================================================
-- VaiTro: 'admin' | 'nhanvien' | 'khachhang'
-- Màn hình đăng nhập dùng cột ID + MatKhau
INSERT INTO Users (ID, HoTen, SDT, BienSo, VaiTro, MatKhau, NgayTao) VALUES
-- ===== ADMIN =====
('admin',   N'Trần Nhật Phước',        '0935955854', '',            'admin',      'admin123', GETDATE()),

-- ===== NHÂN VIÊN =====
('nv01',    N'Trần Ngọc Tấn Lộc',      '0988888888', '',            'nhanvien',   'nv456',    GETDATE()),
('nv02',    N'Nguyễn Phước Minh Quân', '0977777777', '',            'nhanvien',   'nv789',    GETDATE()),
('nv03',    N'Lê Thị Hương',           '0966666666', '',            'nhanvien',   'nv111',    GETDATE()),

-- ===== KHÁCH HÀNG =====
('kh01',    N'Hồ Đại Phong',           '0911223344', '79VA-08761',  'khachhang',  'kh123',    GETDATE()),
('kh02',    N'Trần Thị Anh',           '0909090909', '52-P85748',   'khachhang',  'kh456',    GETDATE()),
('kh03',    N'Lê Văn Bảo',             '0933333333', '51D-57964',   'khachhang',  'kh789',    GETDATE()),
('kh04',    N'Phạm Văn Cường',         '0944444444', '68A-12626',   'khachhang',  'kh321',    GETDATE()),
('kh05',    N'Hoàng Thị Doanh',        '0955555555', '50H-81487',   'khachhang',  'kh654',    GETDATE()),
('kh06',    N'Nguyễn Văn Em',          '0922111333', '43A-56789',   'khachhang',  'kh987',    GETDATE()),
('kh07',    N'Vũ Thị Phương',          '0900123456', '29B-11234',   'khachhang',  'kh111',    GETDATE()),
('kh08',    N'Đặng Minh Tuấn',         '0912345678', '61C-22345',   'khachhang',  'kh222',    GETDATE());
GO

-- ============================================================
-- BƯỚC 5: VỊ TRÍ BÃI ĐỖ XE (32 ô - 4 khu A/B/C/D)
-- ============================================================
-- TrangThai: N'Đang gửi' | N'Trống'
-- Một số ô đang có xe để demo màn hình sơ đồ bãi
INSERT INTO ParkingSlot
    (IDDoXe, KhuVuc, IDNguoiDung, TenKhachHang, SoDienThoai, BienSoXe, ThoiGianVao, MaLoaiXe, HinhAnhVao, TrangThai)
VALUES
-- KHU A: Xe máy (8 ô) — 4 đang gửi, 4 trống
('A1', N'Khu A', 'kh01', N'Hồ Đại Phong',   '0911223344', '79VA-08761', DATEADD(HOUR,-2,GETDATE()),  'LX001', NULL, N'Đang gửi'),
('A2', N'Khu A', 'kh02', N'Trần Thị Anh',   '0909090909', '52-P85748',  DATEADD(HOUR,-5,GETDATE()),  'LX001', NULL, N'Đang gửi'),
('A3', N'Khu A', 'kh03', N'Lê Văn Bảo',     '0933333333', '51D-57964',  DATEADD(HOUR,-1,GETDATE()),  'LX001', NULL, N'Đang gửi'),
('A4', N'Khu A', 'kh06', N'Nguyễn Văn Em',  '0922111333', '43A-56789',  DATEADD(MINUTE,-40,GETDATE()),'LX001', NULL, N'Đang gửi'),
('A5', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A6', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A7', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A8', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),

-- KHU B: Ô tô 4 chỗ (8 ô) — 3 đang gửi, 5 trống
('B1', N'Khu B', 'kh04', N'Phạm Văn Cường', '0944444444', '68A-12626',  DATEADD(HOUR,-8,GETDATE()),  'LX002', NULL, N'Đang gửi'),
('B2', N'Khu B', 'kh05', N'Hoàng Thị Doanh','0955555555', '50H-81487',  DATEADD(HOUR,-3,GETDATE()),  'LX002', NULL, N'Đang gửi'),
('B3', N'Khu B', 'kh07', N'Vũ Thị Phương',  '0900123456', '29B-11234',  DATEADD(HOUR,-1,GETDATE()),  'LX002', NULL, N'Đang gửi'),
('B4', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B5', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B6', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B7', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B8', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),

-- KHU C: Ô tô 7 chỗ + xe tải (8 ô) — 2 đang gửi, 6 trống
('C1', N'Khu C', 'kh08', N'Đặng Minh Tuấn', '0912345678', '61C-22345',  DATEADD(HOUR,-4,GETDATE()),  'LX003', NULL, N'Đang gửi'),
('C2', N'Khu C', 'kh03', N'Lê Văn Bảo',     '0933333333', '51D-57964',  DATEADD(HOUR,-6,GETDATE()),  'LX004', NULL, N'Đang gửi'),
('C3', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C4', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C5', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C6', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('C7', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('C8', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),

-- KHU D: Xe đạp (8 ô) — 1 đang gửi, 7 trống
('D1', N'Khu D', 'kh02', N'Trần Thị Anh',   '0909090909', '52-P85748',  DATEADD(MINUTE,-20,GETDATE()),'LX005', NULL, N'Đang gửi'),
('D2', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D3', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D4', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D5', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D6', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D7', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D8', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống');
GO

-- ============================================================
-- BƯỚC 6: GIAO DỊCH LỊCH SỬ (7 ngày qua)
-- Phục vụ: màn hình Biểu Đồ, Thống Kê, Báo Cáo
-- Logic tính tiền của app:
--   <= 60 phút => 1 giờ
--   > 60 phút  => CEILING(phút/60) giờ
-- ============================================================

INSERT INTO Transactions
    (MaGiaoDich, IDNguoiDung, IDDoXe, TenKhachHang, SoDienThoai, BienSoXe, MaLoaiXe, ThoiGianVao, ThoiGianRa, ThanhTien, HinhAnhVao, HinhAnhRa)
VALUES

-- ============================================================
-- 6 NGÀY TRƯỚC (lịch sử xa)
-- ============================================================
('GD_H6_01', 'kh01', 'A1', N'Hồ Đại Phong',    '0911223344', '79VA-08761', 'LX001',
    DATEADD(HOUR,-6*24+ 7,GETDATE()), DATEADD(HOUR,-6*24+10,GETDATE()), 15000, NULL, NULL),
('GD_H6_02', 'kh04', 'B1', N'Phạm Văn Cường',   '0944444444', '68A-12626',  'LX002',
    DATEADD(HOUR,-6*24+ 8,GETDATE()), DATEADD(HOUR,-6*24+12,GETDATE()), 60000, NULL, NULL),
('GD_H6_03', 'kh07', 'C1', N'Vũ Thị Phương',    '0900123456', '29B-11234',  'LX003',
    DATEADD(HOUR,-6*24+ 9,GETDATE()), DATEADD(HOUR,-6*24+11,GETDATE()), 40000, NULL, NULL),

-- ============================================================
-- 5 NGÀY TRƯỚC
-- ============================================================
('GD_H5_01', 'kh02', 'A2', N'Trần Thị Anh',     '0909090909', '52-P85748',  'LX001',
    DATEADD(HOUR,-5*24+ 6,GETDATE()), DATEADD(HOUR,-5*24+ 8,GETDATE()),  10000, NULL, NULL),
('GD_H5_02', 'kh05', 'B2', N'Hoàng Thị Doanh',  '0955555555', '50H-81487',  'LX002',
    DATEADD(HOUR,-5*24+10,GETDATE()), DATEADD(HOUR,-5*24+14,GETDATE()),  60000, NULL, NULL),
('GD_H5_03', 'kh08', 'C2', N'Đặng Minh Tuấn',   '0912345678', '61C-22345',  'LX004',
    DATEADD(HOUR,-5*24+ 7,GETDATE()), DATEADD(HOUR,-5*24+15,GETDATE()), 240000, NULL, NULL),
('GD_H5_04', 'kh06', 'D1', N'Nguyễn Văn Em',    '0922111333', '43A-56789',  'LX005',
    DATEADD(HOUR,-5*24+ 8,GETDATE()), DATEADD(HOUR,-5*24+ 9,GETDATE()),   2000, NULL, NULL),

-- ============================================================
-- 4 NGÀY TRƯỚC
-- ============================================================
('GD_H4_01', 'kh03', 'A3', N'Lê Văn Bảo',       '0933333333', '51D-57964',  'LX001',
    DATEADD(HOUR,-4*24+ 7,GETDATE()), DATEADD(HOUR,-4*24+ 9,GETDATE()),  10000, NULL, NULL),
('GD_H4_02', 'kh01', 'B3', N'Hồ Đại Phong',     '0911223344', '79VA-08761', 'LX002',
    DATEADD(HOUR,-4*24+ 9,GETDATE()), DATEADD(HOUR,-4*24+13,GETDATE()),  60000, NULL, NULL),
('GD_H4_03', 'kh07', 'B4', N'Vũ Thị Phương',    '0900123456', '29B-11234',  'LX002',
    DATEADD(HOUR,-4*24+11,GETDATE()), DATEADD(HOUR,-4*24+16,GETDATE()),  75000, NULL, NULL),
('GD_H4_04', 'kh04', 'C3', N'Phạm Văn Cường',   '0944444444', '68A-12626',  'LX003',
    DATEADD(HOUR,-4*24+ 8,GETDATE()), DATEADD(HOUR,-4*24+18,GETDATE()), 200000, NULL, NULL),

-- ============================================================
-- 3 NGÀY TRƯỚC
-- ============================================================
('GD_H3_01', 'kh02', 'A1', N'Trần Thị Anh',     '0909090909', '52-P85748',  'LX001',
    DATEADD(HOUR,-3*24+ 6,GETDATE()), DATEADD(HOUR,-3*24+ 7,GETDATE()),   5000, NULL, NULL),
('GD_H3_02', 'kh05', 'A2', N'Hoàng Thị Doanh',  '0955555555', '50H-81487',  'LX001',
    DATEADD(HOUR,-3*24+ 8,GETDATE()), DATEADD(HOUR,-3*24+11,GETDATE()),  15000, NULL, NULL),
('GD_H3_03', 'kh08', 'B1', N'Đặng Minh Tuấn',   '0912345678', '61C-22345',  'LX002',
    DATEADD(HOUR,-3*24+ 9,GETDATE()), DATEADD(HOUR,-3*24+12,GETDATE()),  45000, NULL, NULL),
('GD_H3_04', 'kh06', 'B2', N'Nguyễn Văn Em',    '0922111333', '43A-56789',  'LX002',
    DATEADD(HOUR,-3*24+13,GETDATE()), DATEADD(HOUR,-3*24+17,GETDATE()),  60000, NULL, NULL),
('GD_H3_05', 'kh03', 'C1', N'Lê Văn Bảo',       '0933333333', '51D-57964',  'LX004',
    DATEADD(HOUR,-3*24+ 7,GETDATE()), DATEADD(HOUR,-3*24+19,GETDATE()), 360000, NULL, NULL),
('GD_H3_06', 'kh01', 'D1', N'Hồ Đại Phong',     '0911223344', '79VA-08761', 'LX005',
    DATEADD(HOUR,-3*24+ 7,GETDATE()), DATEADD(HOUR,-3*24+ 8,GETDATE()),   2000, NULL, NULL),

-- ============================================================
-- 2 NGÀY TRƯỚC
-- ============================================================
('GD_H2_01', 'kh04', 'A1', N'Phạm Văn Cường',   '0944444444', '68A-12626',  'LX001',
    DATEADD(HOUR,-2*24+ 7,GETDATE()), DATEADD(HOUR,-2*24+ 9,GETDATE()),  10000, NULL, NULL),
('GD_H2_02', 'kh07', 'A2', N'Vũ Thị Phương',    '0900123456', '29B-11234',  'LX001',
    DATEADD(HOUR,-2*24+10,GETDATE()), DATEADD(HOUR,-2*24+12,GETDATE()),  10000, NULL, NULL),
('GD_H2_03', 'kh05', 'B1', N'Hoàng Thị Doanh',  '0955555555', '50H-81487',  'LX002',
    DATEADD(HOUR,-2*24+ 8,GETDATE()), DATEADD(HOUR,-2*24+16,GETDATE()), 120000, NULL, NULL),
('GD_H2_04', 'kh02', 'B2', N'Trần Thị Anh',     '0909090909', '52-P85748',  'LX002',
    DATEADD(HOUR,-2*24+11,GETDATE()), DATEADD(HOUR,-2*24+14,GETDATE()),  45000, NULL, NULL),
('GD_H2_05', 'kh08', 'C1', N'Đặng Minh Tuấn',   '0912345678', '61C-22345',  'LX003',
    DATEADD(HOUR,-2*24+ 9,GETDATE()), DATEADD(HOUR,-2*24+13,GETDATE()),  80000, NULL, NULL),
('GD_H2_06', 'kh06', 'C2', N'Nguyễn Văn Em',    '0922111333', '43A-56789',  'LX004',
    DATEADD(HOUR,-2*24+ 7,GETDATE()), DATEADD(HOUR,-2*24+20,GETDATE()), 390000, NULL, NULL),
('GD_H2_07', 'kh03', 'D1', N'Lê Văn Bảo',       '0933333333', '51D-57964',  'LX005',
    DATEADD(HOUR,-2*24+ 6,GETDATE()), DATEADD(HOUR,-2*24+ 8,GETDATE()),   4000, NULL, NULL),

-- ============================================================
-- HÔM QUA
-- ============================================================
('GD_H1_01', 'kh01', 'A1', N'Hồ Đại Phong',     '0911223344', '79VA-08761', 'LX001',
    DATEADD(HOUR,-1*24+ 6,GETDATE()), DATEADD(HOUR,-1*24+ 7,GETDATE()),   5000, NULL, NULL),
('GD_H1_02', 'kh02', 'A2', N'Trần Thị Anh',     '0909090909', '52-P85748',  'LX001',
    DATEADD(HOUR,-1*24+ 8,GETDATE()), DATEADD(HOUR,-1*24+11,GETDATE()),  15000, NULL, NULL),
('GD_H1_03', 'kh03', 'A3', N'Lê Văn Bảo',       '0933333333', '51D-57964',  'LX001',
    DATEADD(HOUR,-1*24+12,GETDATE()), DATEADD(HOUR,-1*24+14,GETDATE()),  10000, NULL, NULL),
('GD_H1_04', 'kh04', 'B1', N'Phạm Văn Cường',   '0944444444', '68A-12626',  'LX002',
    DATEADD(HOUR,-1*24+ 7,GETDATE()), DATEADD(HOUR,-1*24+13,GETDATE()),  90000, NULL, NULL),
('GD_H1_05', 'kh05', 'B2', N'Hoàng Thị Doanh',  '0955555555', '50H-81487',  'LX002',
    DATEADD(HOUR,-1*24+14,GETDATE()), DATEADD(HOUR,-1*24+18,GETDATE()),  60000, NULL, NULL),
('GD_H1_06', 'kh06', 'B3', N'Nguyễn Văn Em',    '0922111333', '43A-56789',  'LX002',
    DATEADD(HOUR,-1*24+ 9,GETDATE()), DATEADD(HOUR,-1*24+12,GETDATE()),  45000, NULL, NULL),
('GD_H1_07', 'kh07', 'C1', N'Vũ Thị Phương',    '0900123456', '29B-11234',  'LX003',
    DATEADD(HOUR,-1*24+ 8,GETDATE()), DATEADD(HOUR,-1*24+16,GETDATE()), 160000, NULL, NULL),
('GD_H1_08', 'kh08', 'C2', N'Đặng Minh Tuấn',   '0912345678', '61C-22345',  'LX004',
    DATEADD(HOUR,-1*24+ 7,GETDATE()), DATEADD(HOUR,-1*24+19,GETDATE()), 360000, NULL, NULL),
('GD_H1_09', 'kh01', 'D1', N'Hồ Đại Phong',     '0911223344', '79VA-08761', 'LX005',
    DATEADD(HOUR,-1*24+ 7,GETDATE()), DATEADD(HOUR,-1*24+ 9,GETDATE()),   4000, NULL, NULL),
('GD_H1_10', 'kh03', 'D2', N'Lê Văn Bảo',       '0933333333', '51D-57964',  'LX005',
    DATEADD(HOUR,-1*24+10,GETDATE()), DATEADD(HOUR,-1*24+11,GETDATE()),   2000, NULL, NULL),

-- ============================================================
-- HÔM NAY — giao dịch đã hoàn tất (ThoiGianRa != NULL)
-- Phục vụ màn hình Biểu Đồ & Thống Kê hôm nay
-- ============================================================
('GD_T_01', 'kh01', 'A5', N'Hồ Đại Phong',      '0911223344', '79VA-08761', 'LX001',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 06:30',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 07:30',  5000, NULL, NULL),

('GD_T_02', 'kh02', 'A6', N'Trần Thị Anh',       '0909090909', '52-P85748',  'LX001',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 07:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 09:30', 15000, NULL, NULL),

('GD_T_03', 'kh03', 'A7', N'Lê Văn Bảo',         '0933333333', '51D-57964',  'LX001',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 08:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 10:00', 10000, NULL, NULL),

('GD_T_04', 'kh04', 'B4', N'Phạm Văn Cường',     '0944444444', '68A-12626',  'LX002',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 07:30',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 11:30', 60000, NULL, NULL),

('GD_T_05', 'kh05', 'B5', N'Hoàng Thị Doanh',    '0955555555', '50H-81487',  'LX002',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 09:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 12:00', 45000, NULL, NULL),

('GD_T_06', 'kh06', 'B6', N'Nguyễn Văn Em',      '0922111333', '43A-56789',  'LX002',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 10:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 13:00', 45000, NULL, NULL),

('GD_T_07', 'kh07', 'C3', N'Vũ Thị Phương',      '0900123456', '29B-11234',  'LX003',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 07:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 12:00', 100000, NULL, NULL),

('GD_T_08', 'kh08', 'C4', N'Đặng Minh Tuấn',     '0912345678', '61C-22345',  'LX004',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 06:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 14:00', 240000, NULL, NULL),

('GD_T_09', 'kh02', 'D2', N'Trần Thị Anh',       '0909090909', '52-P85748',  'LX005',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 08:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 09:00',   2000, NULL, NULL),

('GD_T_10', 'kh03', 'D3', N'Lê Văn Bảo',         '0933333333', '51D-57964',  'LX005',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 10:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 12:00',   4000, NULL, NULL),

('GD_T_11', 'kh01', 'A8', N'Hồ Đại Phong',       '0911223344', '79VA-08761', 'LX001',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 13:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 15:30', 15000, NULL, NULL),

('GD_T_12', 'kh04', 'B7', N'Phạm Văn Cường',     '0944444444', '68A-12626',  'LX002',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 14:00',
    CAST(CAST(GETDATE() AS DATE) AS DATETIME) + ' 17:00', 45000, NULL, NULL),

-- ============================================================
-- HÔM NAY — xe đang gửi (ThoiGianRa = NULL)
-- Khớp với ParkingSlot đang gửi → demo CheckOut
-- ============================================================
('GD_T_CX1', 'kh01', 'A1', N'Hồ Đại Phong',     '0911223344', '79VA-08761', 'LX001',
    DATEADD(HOUR,-2,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX2', 'kh02', 'A2', N'Trần Thị Anh',      '0909090909', '52-P85748',  'LX001',
    DATEADD(HOUR,-5,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX3', 'kh03', 'A3', N'Lê Văn Bảo',        '0933333333', '51D-57964',  'LX001',
    DATEADD(HOUR,-1,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX4', 'kh06', 'A4', N'Nguyễn Văn Em',     '0922111333', '43A-56789',  'LX001',
    DATEADD(MINUTE,-40,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX5', 'kh04', 'B1', N'Phạm Văn Cường',    '0944444444', '68A-12626',  'LX002',
    DATEADD(HOUR,-8,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX6', 'kh05', 'B2', N'Hoàng Thị Doanh',   '0955555555', '50H-81487',  'LX002',
    DATEADD(HOUR,-3,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX7', 'kh07', 'B3', N'Vũ Thị Phương',     '0900123456', '29B-11234',  'LX002',
    DATEADD(HOUR,-1,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX8', 'kh08', 'C1', N'Đặng Minh Tuấn',    '0912345678', '61C-22345',  'LX003',
    DATEADD(HOUR,-4,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX9', 'kh03', 'C2', N'Lê Văn Bảo',        '0933333333', '51D-57964',  'LX004',
    DATEADD(HOUR,-6,GETDATE()), NULL, 0, NULL, NULL),

('GD_T_CX10','kh02', 'D1', N'Trần Thị Anh',       '0909090909', '52-P85748',  'LX005',
    DATEADD(MINUTE,-20,GETDATE()), NULL, 0, NULL, NULL);
GO

-- ============================================================
-- BƯỚC 7: KIỂM TRA DỮ LIỆU
-- ============================================================

-- Tổng hợp theo bảng
SELECT 'LoaiXe'      AS Bang, COUNT(*) AS SoLuong FROM LoaiXe     UNION ALL
SELECT 'Users'       AS Bang, COUNT(*) AS SoLuong FROM Users       UNION ALL
SELECT 'ParkingSlot' AS Bang, COUNT(*) AS SoLuong FROM ParkingSlot UNION ALL
SELECT 'Transactions'AS Bang, COUNT(*) AS SoLuong FROM Transactions;

-- Trạng thái bãi xe
SELECT TrangThai, COUNT(*) AS SoO FROM ParkingSlot GROUP BY TrangThai;

-- Doanh thu hôm nay
SELECT
    COUNT(*)       AS SoGiaoDichHomNay,
    SUM(ThanhTien) AS DoanhThuHomNay
FROM Transactions
WHERE CAST(ThoiGianRa AS DATE) = CAST(GETDATE() AS DATE);

-- Doanh thu 7 ngày (dùng cho biểu đồ)
SELECT
    CAST(ThoiGianRa AS DATE) AS Ngay,
    COUNT(*)                 AS SoLuot,
    SUM(ThanhTien)           AS DoanhThu
FROM Transactions
WHERE ThoiGianRa IS NOT NULL
  AND ThoiGianRa >= DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
GROUP BY CAST(ThoiGianRa AS DATE)
ORDER BY Ngay;

-- Xe đang gửi hiện tại
SELECT IDDoXe, KhuVuc, TenKhachHang, BienSoXe, ThoiGianVao, MaLoaiXe
FROM ParkingSlot
WHERE TrangThai = N'Đang gửi'
ORDER BY KhuVuc, IDDoXe;
GO

-- ============================================================
-- THÔNG TIN ĐĂNG NHẬP DEMO
-- ============================================================
/*
┌────────────────────────────────────────────────────────┐
│             TÀI KHOẢN ĐĂNG NHẬP DEMO                  │
├──────────────┬──────────────┬──────────────────────────┤
│ Tên đăng nhập│  Mật khẩu   │ Vai trò                  │
├──────────────┼──────────────┼──────────────────────────┤
│ admin        │ admin123     │ Quản trị viên (full)     │
│ nv01         │ nv456        │ Nhân viên (không quản lý)│
│ nv02         │ nv789        │ Nhân viên                │
│ kh01         │ kh123        │ Khách hàng (tìm kiếm)   │
│ kh02         │ kh456        │ Khách hàng               │
└──────────────┴──────────────┴──────────────────────────┘

TÌNH TRẠNG BÃI XE:
  - Tổng ô: 32  |  Đang gửi: 10  |  Trống: 22

DOANH THU HÔM NAY (ước tính): ~586,000đ / 12 lượt xe
DOANH THU 7 NGÀY: xem kết quả SELECT bên trên

CÁC TÍNH NĂNG CÓ THỂ DEMO:
  [admin]
  ✔ Đăng nhập, đăng xuất
  ✔ Xem sơ đồ bãi xe (A1-D8, màu đỏ/xanh)
  ✔ Check-in xe vào ô trống (A5-A8, B4-B8, C3-C8, D2-D8)
  ✔ Check-out xe đang gửi (A1-A4, B1-B3, C1-C2, D1)
  ✔ Tìm kiếm xe theo biển số
  ✔ Quản lý nhân sự (thêm/sửa/xóa nhân viên)
  ✔ Quản lý loại xe (thêm/sửa/xóa)
  ✔ Biểu đồ & thống kê doanh thu
  ✔ Dashboard tổng quan
  ✔ Tài khoản cá nhân, đổi mật khẩu

  [nv01 / nv02]
  ✔ Sơ đồ bãi xe, check-in, check-out
  ✔ Tìm kiếm xe, báo cáo
  ✗ Không thấy menu Quản lý nhân sự, Loại xe

  [kh01 / kh02]
  ✔ Chỉ thấy màn hình Tìm kiếm xe
  ✔ Tài khoản cá nhân
*/