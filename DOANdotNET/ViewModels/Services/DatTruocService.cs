using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DOANdotNET.Models;

namespace DOANdotNET.ViewModels.Services
{
    public class DatTruocService
    {
        // FIX: Dùng EF context để lấy connection string
        // → Không cần đọc App.config thủ công, không bao giờ bị lỗi "not found"
        // → EF tự biết connection string vì nó đã dùng được ở ParkingService
        private static string _cachedConnStr = null;

        private string GetSqlConnectionString()
        {
            if (_cachedConnStr != null) return _cachedConnStr;
            using (var db = new QL_BaiDoXeEntities())
            {
                // Lấy connection string thuần SQL từ EF context
                _cachedConnStr = db.Database.Connection.ConnectionString;
                return _cachedConnStr;
            }
        }

        private SqlConnection OpenConn()
        {
            var conn = new SqlConnection(GetSqlConnectionString());
            conn.Open();
            return conn;
        }

        private DatTruoc DocDong(SqlDataReader rdr)
        {
            return new DatTruoc
            {
                MaDatTruoc = rdr["MaDatTruoc"].ToString(),
                IDKhachHang = rdr["IDKhachHang"].ToString(),
                KhuVuc = rdr["KhuVuc"].ToString(),
                MaLoaiXe = rdr["MaLoaiXe"].ToString(),
                ThoiGianDen = (DateTime)rdr["ThoiGianDen"],
                ThoiGianHetHan = (DateTime)rdr["ThoiGianHetHan"],
                TrangThai = rdr["TrangThai"].ToString(),
                IDDoXe = rdr["IDDoXe"] as string,
                GhiChu = rdr["GhiChu"] as string,
                NgayTao = (DateTime)rdr["NgayTao"],
                GiaDoXe = rdr["GiaDoXe"] as decimal?,
                LoaiXe = new LoaiXe
                {
                    TenLoaiXe = rdr["TenLoaiXe"]?.ToString(),
                    PhiMoiGio = rdr["GiaDoXe"] as decimal? ?? 0
                },
                User = new User
                {
                    HoTen = rdr["TenKhach"]?.ToString()
                }
            };
        }

        public string TaoDatTruoc(string idKhachHang, string khuVuc,
                                   string maLoaiXe, DateTime thoiGianDen,
                                   string ghiChu = null)
        {
            using (var conn = OpenConn())
            {
                string idDoXe = null;
                using (var cmd = new SqlCommand(
                    @"SELECT TOP 1 IDDoXe FROM ParkingSlot
                      WHERE KhuVuc = @khu AND TrangThai = N'Trống'
                      ORDER BY IDDoXe", conn))
                {
                    cmd.Parameters.AddWithValue("@khu", khuVuc);
                    var val = cmd.ExecuteScalar();
                    if (val == null) return null;
                    idDoXe = val.ToString();
                }

                string ma = "DT-" + DateTime.Now.ToString("yyMMddHHmmss")
                                   + "-" + idKhachHang.ToUpper();
                DateTime hetHan = thoiGianDen.AddMinutes(30);

                using (var cmd = new SqlCommand(
                    @"INSERT INTO DatTruoc
                        (MaDatTruoc,IDKhachHang,KhuVuc,MaLoaiXe,
                         ThoiGianDen,ThoiGianHetHan,TrangThai,IDDoXe,GhiChu,NgayTao)
                      VALUES
                        (@ma,@kh,@khu,@lx,@den,@het,N'Chờ xử lý',@slot,@ghiChu,GETDATE())", conn))
                {
                    cmd.Parameters.AddWithValue("@ma", ma);
                    cmd.Parameters.AddWithValue("@kh", idKhachHang);
                    cmd.Parameters.AddWithValue("@khu", khuVuc);
                    cmd.Parameters.AddWithValue("@lx", maLoaiXe);
                    cmd.Parameters.AddWithValue("@den", thoiGianDen);
                    cmd.Parameters.AddWithValue("@het", hetHan);
                    cmd.Parameters.AddWithValue("@slot", idDoXe);
                    cmd.Parameters.AddWithValue("@ghiChu", (object)ghiChu ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                return ma;
            }
        }

        public bool HuyDatTruoc(string maDatTruoc, string idKhachHang)
        {
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"UPDATE DatTruoc SET TrangThai = N'Đã hủy'
                  WHERE MaDatTruoc = @ma AND IDKhachHang = @kh
                    AND TrangThai IN (N'Chờ xử lý',N'Đã xác nhận')", conn))
            {
                cmd.Parameters.AddWithValue("@ma", maDatTruoc);
                cmd.Parameters.AddWithValue("@kh", idKhachHang);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool HuyBoiAdmin(string maDatTruoc)
        {
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"UPDATE DatTruoc SET TrangThai = N'Đã hủy'
                  WHERE MaDatTruoc = @ma
                    AND TrangThai IN (N'Chờ xử lý',N'Đã xác nhận')", conn))
            {
                cmd.Parameters.AddWithValue("@ma", maDatTruoc);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XacNhanDatTruoc(string maDatTruoc)
        {
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"UPDATE DatTruoc SET TrangThai = N'Đã xác nhận'
                  WHERE MaDatTruoc = @ma AND TrangThai = N'Chờ xử lý'", conn))
            {
                cmd.Parameters.AddWithValue("@ma", maDatTruoc);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<DatTruoc> LayLichSu(string idKhachHang)
        {
            var list = new List<DatTruoc>();
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"SELECT d.*, l.TenLoaiXe, l.PhiMoiGio AS GiaDoXe, NULL AS TenKhach
                  FROM DatTruoc d
                  LEFT JOIN LoaiXe l ON l.MaLoaiXe = d.MaLoaiXe
                  WHERE d.IDKhachHang = @kh
                  ORDER BY d.NgayTao DESC", conn))
            {
                cmd.Parameters.AddWithValue("@kh", idKhachHang);
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(DocDong(rdr));
            }
            return list;
        }

        public List<DatTruoc> LayTatCa(string trangThaiFilter = null)
        {
            var list = new List<DatTruoc>();
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"SELECT d.*, l.TenLoaiXe, l.PhiMoiGio AS GiaDoXe, u.HoTen AS TenKhach
                  FROM DatTruoc d
                  LEFT JOIN LoaiXe l ON l.MaLoaiXe = d.MaLoaiXe
                  LEFT JOIN Users  u ON u.ID = d.IDKhachHang
                  WHERE (@tt IS NULL OR d.TrangThai = @tt)
                  ORDER BY d.NgayTao DESC", conn))
            {
                cmd.Parameters.AddWithValue("@tt", (object)trangThaiFilter ?? DBNull.Value);
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(DocDong(rdr));
            }
            return list;
        }

        public List<string> LayKhuVucConTrong()
        {
            var list = new List<string>();
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"SELECT DISTINCT KhuVuc FROM ParkingSlot
                  WHERE TrangThai = N'Trống' ORDER BY KhuVuc", conn))
            using (var rdr = cmd.ExecuteReader())
                while (rdr.Read()) list.Add(rdr[0].ToString());
            return list;
        }

        public int DemSlotTrong(string khuVuc)
        {
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"SELECT COUNT(*) FROM ParkingSlot
                  WHERE KhuVuc = @khu AND TrangThai = N'Trống'", conn))
            {
                cmd.Parameters.AddWithValue("@khu", khuVuc);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int DemDangDatTruoc()
        {
            using (var conn = OpenConn())
            using (var cmd = new SqlCommand(
                @"SELECT COUNT(*) FROM DatTruoc
                  WHERE TrangThai IN (N'Chờ xử lý',N'Đã xác nhận')
                    AND ThoiGianHetHan >= GETDATE()", conn))
                return (int)cmd.ExecuteScalar();
        }
    }
}