using System;
using System.Collections.Generic;
using System.Linq;
using DOANdotNET.Models;

namespace DOANdotNET.ViewModels.Services
{
    public class ParkingService
    {
        // ===== XÁC THỰC =====
        public User Authenticate(string id, string pw)
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.Users.FirstOrDefault(u => u.ID == id && u.MatKhau == pw);
        }

        // ===== LOẠI XE =====
        public List<LoaiXe> GetAllLoaiXe()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.LoaiXes.ToList();
        }

        public void AddLoaiXe(LoaiXe lx)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                if (db.LoaiXes.Any(l => l.MaLoaiXe == lx.MaLoaiXe)) return;
                db.LoaiXes.Add(lx);
                db.SaveChanges();
            }
        }

        public void RemoveLoaiXe(string ma)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var lx = db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == ma);
                if (lx == null) return;
                db.LoaiXes.Remove(lx);
                db.SaveChanges();
            }
        }

        public void UpdateLoaiXe(LoaiXe updated)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var lx = db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == updated.MaLoaiXe);
                if (lx == null) return;
                lx.TenLoaiXe = updated.TenLoaiXe;
                lx.PhiMoiGio = updated.PhiMoiGio;
                db.SaveChanges();
            }
        }

        // ===== PARKING SLOT =====
        public List<ParkingSlot> GetParkingSlots()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.ParkingSlots.ToList();
        }

        public ParkingSlot FindByBienSo(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo)) return null;
            string key = bienSo.Replace("-", "").Replace(" ", "").ToUpper();
            using (var db = new QL_BaiDoXeEntities())
            {
                var slots = db.ParkingSlots
                    .Where(s => s.TrangThai == "Đang gửi" && s.BienSoXe != null)
                    .ToList();
                return slots.FirstOrDefault(s =>
                    s.BienSoXe.Replace("-", "").Replace(" ", "").ToUpper() == key);
            }
        }

        public int CountTrong()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.ParkingSlots.Count(s => s.TrangThai == "Trống");
        }

        public int CountCoXe()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.ParkingSlots.Count(s => s.TrangThai == "Đang gửi");
        }

        public int CountTongSlot()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.ParkingSlots.Count();
        }

        public bool AddSlot(string idDoXe, string khuVuc)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                if (db.ParkingSlots.Any(s => s.IDDoXe == idDoXe)) return false;
                db.ParkingSlots.Add(new ParkingSlot
                {
                    IDDoXe = idDoXe.ToUpper(),
                    KhuVuc = khuVuc,
                    TrangThai = "Trống"
                });
                db.SaveChanges();
                return true;
            }
        }

        public bool RemoveSlot(string idDoXe)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var slot = db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
                if (slot == null || slot.TrangThai == "Đang gửi") return false;
                db.ParkingSlots.Remove(slot);
                db.SaveChanges();
                return true;
            }
        }

        // ===== CHECK IN =====
        public bool CheckIn(string idDoXe, string tenKhach, string sdt,
                            string bienSo, string maLoaiXe = "LX001",
                            string hinhAnh = null)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var slot = db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
                if (slot == null || slot.TrangThai != "Trống") return false;

                slot.TrangThai = "Đang gửi";
                slot.TenKhachHang = tenKhach;
                slot.SoDienThoai = sdt;
                slot.BienSoXe = bienSo;
                slot.MaLoaiXe = maLoaiXe;
                slot.HinhAnhVao = hinhAnh;
                slot.ThoiGianVao = DateTime.Now;

                db.Transactions.Add(new Transaction
                {
                    MaGiaoDich = "GD" + DateTime.Now.ToString("yyMMddHHmmss") + idDoXe,
                    IDDoXe = idDoXe,
                    IDNguoiDung = slot.IDNguoiDung ?? "admin",
                    TenKhachHang = tenKhach,
                    SoDienThoai = sdt,
                    BienSoXe = bienSo,
                    MaLoaiXe = maLoaiXe,
                    ThoiGianVao = slot.ThoiGianVao.Value,
                    ThoiGianRa = null,
                    ThanhTien = 0,
                    HinhAnhVao = hinhAnh
                });

                db.SaveChanges();
                return true;
            }
        }

        // ===== CHECK OUT =====
        public decimal CheckOut(string idDoXe, out string tenKhachOut)
        {
            tenKhachOut = "";
            using (var db = new QL_BaiDoXeEntities())
            {
                var slot = db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
                if (slot == null || slot.TrangThai == "Trống") return 0m;

                var loaiXe = db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == slot.MaLoaiXe);
                decimal phiGio = loaiXe != null ? (decimal)loaiXe.PhiMoiGio : 10000m;

                decimal tien = 0m;
                if (slot.ThoiGianVao.HasValue)
                {
                    double phut = (DateTime.Now - slot.ThoiGianVao.Value).TotalMinutes;
                    tien = phut <= 60
                        ? phiGio
                        : (decimal)Math.Ceiling(phut / 60.0) * phiGio;
                }

                var trans = db.Transactions
                    .FirstOrDefault(t => t.IDDoXe == idDoXe && t.ThoiGianRa == null);
                if (trans != null)
                {
                    trans.ThoiGianRa = DateTime.Now;
                    trans.ThanhTien = tien;
                }
                else
                {
                    db.Transactions.Add(new Transaction
                    {
                        MaGiaoDich = "GD" + DateTime.Now.ToString("yyMMddHHmmss") + idDoXe,
                        IDDoXe = idDoXe,
                        IDNguoiDung = slot.IDNguoiDung ?? "admin",
                        MaLoaiXe = slot.MaLoaiXe ?? "LX001",
                        TenKhachHang = slot.TenKhachHang,
                        SoDienThoai = slot.SoDienThoai,
                        BienSoXe = slot.BienSoXe,
                        ThoiGianVao = slot.ThoiGianVao ?? DateTime.Now,
                        ThoiGianRa = DateTime.Now,
                        ThanhTien = tien
                    });
                }

                tenKhachOut = slot.TenKhachHang;
                slot.TrangThai = "Trống";
                slot.TenKhachHang = null;
                slot.SoDienThoai = null;
                slot.BienSoXe = null;
                slot.MaLoaiXe = null;
                slot.HinhAnhVao = null;
                slot.IDNguoiDung = null;
                slot.ThoiGianVao = null;

                db.SaveChanges();
                return tien;
            }
        }

        // ===== QUẢN LÝ USER =====
        public List<User> GetAllUsers()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.Users.ToList();
        }

        public User GetUserByID(string id)
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.Users.FirstOrDefault(u => u.ID == id);
        }

        public bool AddUser(User user)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                if (db.Users.Any(u => u.ID == user.ID)) return false;
                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
        }

        public void RemoveUser(string id)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var u = db.Users.FirstOrDefault(x => x.ID == id);
                if (u == null) return;
                db.Users.Remove(u);
                db.SaveChanges();
            }
        }

        public void UpdateUser(User updated)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var u = db.Users.FirstOrDefault(x => x.ID == updated.ID);
                if (u == null) return;
                u.HoTen = updated.HoTen;
                u.MatKhau = updated.MatKhau;
                u.SDT = updated.SDT;
                u.BienSo = updated.BienSo;
                u.VaiTro = updated.VaiTro;
                db.SaveChanges();
            }
        }

        public bool DoiMatKhau(string id, string matKhauCu, string matKhauMoi)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var u = db.Users.FirstOrDefault(x => x.ID == id && x.MatKhau == matKhauCu);
                if (u == null) return false;
                u.MatKhau = matKhauMoi;
                db.SaveChanges();
                return true;
            }
        }

        // ===== BÁO CÁO =====
        public List<Transaction> GetTransactions()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.Transactions.ToList();
        }

        public decimal GetTotalRevenue()
        {
            using (var db = new QL_BaiDoXeEntities())
                return db.Transactions.Any()
                    ? db.Transactions.Sum(t => t.ThanhTien)
                    : 0;
        }

        // FIX: Lọc theo ThoiGianRa thay vì ThoiGianVao
        // → khớp với GetDoanhThuHomNay() của Dashboard
        // → chỉ tính xe đã checkout (có tiền thật), không tính xe đang đỗ
        public List<Transaction> GetTransactionsByDate(DateTime tuNgay, DateTime denNgay)
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var denNgayCuoi = denNgay.Date.AddDays(1).AddTicks(-1);
                return db.Transactions
                    .Where(t => t.ThoiGianRa != null
                             && t.ThoiGianRa >= tuNgay.Date
                             && t.ThoiGianRa <= denNgayCuoi)
                    .ToList();
            }
        }

        public decimal TinhPhiDuTinh(ParkingSlot slot)
        {
            if (!slot.ThoiGianVao.HasValue) return 0;
            using (var db = new QL_BaiDoXeEntities())
            {
                var loaiXe = db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == slot.MaLoaiXe);
                decimal phiGio = loaiXe != null ? (decimal)loaiXe.PhiMoiGio : 10000m;
                double phut = (DateTime.Now - slot.ThoiGianVao.Value).TotalMinutes;
                double gio = phut <= 60 ? 1 : Math.Ceiling(phut / 60.0);
                return (decimal)gio * phiGio;
            }
        }

        // ===== DASHBOARD =====
        public decimal GetDoanhThuHomNay()
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var homNay = DateTime.Today;
                return db.Transactions
                    .Where(t => t.ThoiGianRa != null &&
                           System.Data.Entity.DbFunctions
                               .TruncateTime(t.ThoiGianRa) == homNay)
                    .Sum(t => (decimal?)t.ThanhTien) ?? 0;
            }
        }

        public int GetLuotRaHomNay()
        {
            using (var db = new QL_BaiDoXeEntities())
            {
                var homNay = DateTime.Today;
                return db.Transactions
                    .Count(t => t.ThoiGianRa != null &&
                           System.Data.Entity.DbFunctions
                               .TruncateTime(t.ThoiGianRa) == homNay);
            }
        }
    }
}