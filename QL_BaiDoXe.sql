
CREATE TABLE LoaiXe (
    MaLoaiXe VARCHAR(20) NOT NULL PRIMARY KEY,
    TenLoaiXe NVARCHAR(50) NOT NULL,
    PhiMoiGio DECIMAL(18,0) NOT NULL DEFAULT 0
 
);


CREATE TABLE Users (
    ID VARCHAR(50) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SDT VARCHAR(15) NULL,
    BienSo VARCHAR(20) NULL,
    VaiTro VARCHAR(20) NOT NULL,
    MatKhau VARCHAR(255) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE() 
);


CREATE TABLE ParkingSlot (
    IDDoXe VARCHAR(20) NOT NULL PRIMARY KEY,
    KhuVuc NVARCHAR(50) NOT NULL,
    IDNguoiDung VARCHAR(50) NULL FOREIGN KEY REFERENCES Users(ID),
    TenKhachHang NVARCHAR(100) NULL,
    SoDienThoai VARCHAR(15) NULL,
    BienSoXe VARCHAR(20) NULL,
    ThoiGianVao DATETIME NULL,
    MaLoaiXe VARCHAR(20) NULL FOREIGN KEY REFERENCES LoaiXe(MaLoaiXe),
    HinhAnhVao NVARCHAR(MAX) NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Trong'
);


CREATE TABLE Transactions (
    MaGiaoDich VARCHAR(50) NOT NULL PRIMARY KEY,
    IDNguoiDung VARCHAR(50) NULL FOREIGN KEY REFERENCES Users(ID),
    IDDoXe VARCHAR(20) NULL,
    TenKhachHang NVARCHAR(100) NULL,
    SoDienThoai VARCHAR(15) NULL,
    BienSoXe VARCHAR(20) NOT NULL,
    MaLoaiXe VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES LoaiXe(MaLoaiXe),
    ThoiGianVao DATETIME NOT NULL,
    ThoiGianRa DATETIME NULL,
    ThanhTien DECIMAL(18,0) NOT NULL DEFAULT 0,
    HinhAnhVao NVARCHAR(MAX) NULL,
    HinhAnhRa NVARCHAR(MAX) NULL
);

INSERT INTO LoaiXe (MaLoaiXe, TenLoaiXe, PhiMoiGio) 
VALUES 
('LX001', N'Xe máy', 5000),
('LX002', N'Xe ô tô 4 chỗ', 15000),
('LX003', N'Xe ô tô 7 chỗ', 20000),
('LX004', N'Xe tải nhỏ', 30000),
('LX005', N'Xe đạp', 2000);


INSERT INTO Users (ID, HoTen, SDT, BienSo, VaiTro, MatKhau, NgayTao) VALUES 
('admin', N'Trần Nhật Phước', '0935955854', '', 'admin', 'admin123', GETDATE()),
('nv01', N'Trần Ngọc Tấn Lộc', '0988888888', '', 'nhanvien', 'nv456', GETDATE()),
('nv02', N'Nguyễn Phước Minh Quân', '0977777777', '', 'nhanvien', 'nv789', GETDATE()),
('kh01', N'Hồ Đại Phong', '0911223344', '79VA-08761', 'khachhang', 'kh123', GETDATE()),
('kh02', N'Trần Thị Anh', '0909090909', '52-P85748', 'khachhang', 'kh456', GETDATE()),
('kh03', N'Lê Văn Bảo', '0933333333', '51D-57964', 'khachhang', 'kh789', GETDATE()),
('kh04', N'Phạm Văn Cường', '0944444444', '68A-12626', 'khachhang', 'kh321', GETDATE()),
('kh05', N'Hoàng Thị Doanh', '0955555555', '50H-81487', 'khachhang', 'kh654', GETDATE());


INSERT INTO ParkingSlot (IDDoXe, KhuVuc, IDNguoiDung, TenKhachHang, SoDienThoai, BienSoXe, ThoiGianVao, MaLoaiXe, HinhAnhVao, TrangThai) VALUES 
('A1', N'Khu A', 'kh01', N'Hồ Đại Phong', '0911223344', '79VA-08761', DATEADD(HOUR, -2, GETDATE()), 'LX001', 'IMG/vxm1.jpg', N'Đang gửi'),
('A2', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A3', N'Khu A', 'kh02', N'Trần Thị Anh', '0909090909', '52-P85748', DATEADD(HOUR, -5, GETDATE()), 'LX001', 'IMG/vxm3.jpg', N'Đang gửi'),
('A4', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A5', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A6', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A7', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('A8', N'Khu A', NULL, NULL, NULL, NULL, NULL, 'LX001', NULL, N'Trống'),
('B1', N'Khu B', 'kh04', N'Phạm Văn Cường', '0944444444', '68A-12626', DATEADD(HOUR, -8, GETDATE()), 'LX002', 'IMG/vot1.jpg', N'Đang gửi'),
('B2', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B3', N'Khu B', 'kh05', N'Hoàng Thị Doanh', '0955555555', '50H-81487', DATEADD(HOUR, -3, GETDATE()), 'LX002', 'IMG/vot2.jpg', N'Đang gửi'),
('B4', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B5', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B6', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B7', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('B8', N'Khu B', NULL, NULL, NULL, NULL, NULL, 'LX002', NULL, N'Trống'),
('C1', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C2', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C3', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C4', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX003', NULL, N'Trống'),
('C5', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('C6', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('C7', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('C8', N'Khu C', NULL, NULL, NULL, NULL, NULL, 'LX004', NULL, N'Trống'),
('D1', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D2', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D3', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D4', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D5', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D6', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D7', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống'),
('D8', N'Khu D', NULL, NULL, NULL, NULL, NULL, 'LX005', NULL, N'Trống');


INSERT INTO Transactions (MaGiaoDich, IDNguoiDung, IDDoXe, TenKhachHang, SoDienThoai, BienSoXe, MaLoaiXe, ThoiGianVao, ThoiGianRa, ThanhTien, HinhAnhVao, HinhAnhRa) VALUES 
-- 5 GIAO DỊCH ĐÃ XONG 
('GD001', 'kh01', 'A1', N'Hồ Đại Phong', '0911223344', '79VA-08761', 'LX001', DATEADD(HOUR, -10, GETDATE()), DATEADD(HOUR, -8, GETDATE()), 10000, 'IMG/vxm1.jpg', 'IMG/rxm1.jpg'),
('GD002', 'kh04', 'B1', N'Phạm Văn Cường', '0944444444', '68A-12626', 'LX002', DATEADD(HOUR, -24, GETDATE()), DATEADD(HOUR, -20, GETDATE()), 60000, 'IMG/vot1.jpg', 'IMG/rot1.jpg'),
('GD003', 'kh02', 'A3', N'Trần Thị Anh', '0909090909', '52-P85748', 'LX001', DATEADD(HOUR, -5, GETDATE()), DATEADD(HOUR, -4, GETDATE()), 5000, 'IMG/vxm3.jpg', 'IMG/rxm3.jpg'),
('GD004', 'kh05', 'B3', N'Hoàng Thị Doanh', '0955555555', '50H-81487', 'LX002', DATEADD(HOUR, -48, GETDATE()), DATEADD(HOUR, -46, GETDATE()), 30000, 'IMG/vot2.jpg', 'IMG/rot2.jpg'),
('GD005', 'kh03', 'C1', N'Lê Văn Bảo', '0933333333', '51D-57964', 'LX003', DATEADD(HOUR, -12, GETDATE()), DATEADD(HOUR, -9, GETDATE()), 60000, 'IMG/vx7c1.jpg', 'IMG/rx7c1.jpg'),

-- 4 GIAO DỊCH ĐANG GỬI 
('GD006', 'kh01', 'A1', N'Hồ Đại Phong', '0911223344', '79VA-08761', 'LX001', DATEADD(HOUR, -2, GETDATE()), NULL, 0, 'IMG/vxm1.jpg', NULL),
('GD007', 'kh02', 'A3', N'Trần Thị Anh', '0909090909', '52-P85748', 'LX001', DATEADD(HOUR, -5, GETDATE()), NULL, 0, 'IMG/vxm3.jpg', NULL),
('GD008', 'kh04', 'B1', N'Phạm Văn Cường', '0944444444', '68A-12626', 'LX002', DATEADD(HOUR, -8, GETDATE()), NULL, 0, 'IMG/vot1.jpg', NULL),
('GD009', 'kh05', 'B3', N'Hoàng Thị Doanh', '0955555555', '50H-81487', 'LX002', DATEADD(HOUR, -3, GETDATE()), NULL, 0, 'IMG/vot2.jpg', NULL);

SELECT COUNT(*) FROM ParkingSlot WHERE MaLoaiXe = 'LX001' AND TrangThai = N'Đang gửi';

SELECT * FROM Users;
SELECT * FROM LoaiXe;
SELECT * FROM Transactions;
SELECT * FROM ParkingSlot;
delete from users;
delete from LoaiXe;
delete from ParkingSlot
delete from Transactions;