using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.Models;

namespace DOANdotNET.ViewModels.Services
{
    public class ParkingService
    {
        // Khởi tạo cầu nối trực tiếp đến CSDL
        // TODO: Nên dùng using() hoặc DI để quản lý lifetime của DbContext
        private QL_BaiDoXeEntities _db = new QL_BaiDoXeEntities();

        public ParkingService()
        {
        }

        // ===== XÁC THỰC =====
        public User Authenticate(string id, string pw)
        {
            // TODO: Nên hash mật khẩu bằng BCrypt/SHA256 trước khi so sánh (demo chưa implement)
            return _db.Users.FirstOrDefault(u => u.ID == id && u.MatKhau == pw);
        }

        // ===== LOẠI XE =====
        public List<LoaiXe> GetAllLoaiXe() => _db.LoaiXes.ToList();

        public void AddLoaiXe(LoaiXe lx)
        {
            if (!_db.LoaiXes.Any(l => l.MaLoaiXe == lx.MaLoaiXe))
            {
                _db.LoaiXes.Add(lx);
                _db.SaveChanges();
            }
        }

        public void RemoveLoaiXe(string ma)
        {
            var lx = _db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == ma);
            if (lx != null)
            {
                _db.LoaiXes.Remove(lx);
                _db.SaveChanges();
            }
        }

        public void UpdateLoaiXe(LoaiXe updated)
        {
            var lx = _db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == updated.MaLoaiXe);
            if (lx == null) return;
            lx.TenLoaiXe = updated.TenLoaiXe;
            lx.PhiMoiGio = updated.PhiMoiGio;
            _db.SaveChanges();
        }

        // ===== PARKING SLOT =====
        public List<ParkingSlot> GetParkingSlots() => _db.ParkingSlots.ToList();

        public ParkingSlot FindByBienSo(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo)) return null;
            string key = bienSo.Replace("-", "").Replace(" ", "").ToUpper();

            var slots = _db.ParkingSlots.Where(s => s.TrangThai == "Đang gửi" && s.BienSoXe != null).ToList();
            return slots.FirstOrDefault(s => s.BienSoXe.Replace("-", "").Replace(" ", "").ToUpper() == key);
        }

        public int CountTrong() => _db.ParkingSlots.Count(s => s.TrangThai == "Trống");
        public int CountCoXe() => _db.ParkingSlots.Count(s => s.TrangThai == "Đang gửi");

        public bool AddSlot(string idDoXe, string khuVuc)
        {
            if (_db.ParkingSlots.Any(s => s.IDDoXe == idDoXe)) return false;
            _db.ParkingSlots.Add(new ParkingSlot
            {
                IDDoXe = idDoXe.ToUpper(),
                KhuVuc = khuVuc,
                TrangThai = "Trống"
            });
            _db.SaveChanges();
            return true;
        }

        public bool RemoveSlot(string idDoXe)
        {
            var slot = _db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
            if (slot == null || slot.TrangThai == "Đang gửi") return false;
            _db.ParkingSlots.Remove(slot);
            _db.SaveChanges();
            return true;
        }

        // ===== CHECK IN =====
        public bool CheckIn(string idDoXe, string tenKhach, string sdt, string bienSo, string maLoaiXe = "LX001", string hinhAnh = null)
        {
            var slot = _db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
            if (slot == null || slot.TrangThai != "Trống") return false;

            slot.TrangThai = "Đang gửi";
            slot.TenKhachHang = tenKhach;
            slot.SoDienThoai = sdt;
            slot.BienSoXe = bienSo;
            slot.MaLoaiXe = maLoaiXe;
            slot.HinhAnhVao = hinhAnh;
            slot.ThoiGianVao = DateTime.Now;

            // Ghi một phiếu rỗng vào sổ Transactions
            _db.Transactions.Add(new Transaction
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

            _db.SaveChanges(); // Cập nhật cả 2 bảng cùng lúc
            return true;
        }

        // ===== CHECK OUT =====
        public decimal CheckOut(string idDoXe, out string tenKhachOut)
        {
            tenKhachOut = "";
            var slot = _db.ParkingSlots.FirstOrDefault(s => s.IDDoXe == idDoXe);
            if (slot == null || slot.TrangThai == "Trống") return 0m;

            var loaiXe = _db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == slot.MaLoaiXe);
            decimal phiGio = loaiXe != null ? (decimal)loaiXe.PhiMoiGio : 10000m;

            decimal tien = 0m;
            if (slot.ThoiGianVao.HasValue)
            {
                double phut = (DateTime.Now - slot.ThoiGianVao.Value).TotalMinutes;
                if (phut <= 60)
                    tien = phiGio;
                else
                {
                    double gio = Math.Ceiling(phut / 60.0);
                    tien = (decimal)gio * phiGio;
                }
            }

            // TÌM LẠI PHIẾU GIAO DỊCH KHI XE VÀO ĐỂ ĐIỀN NỐT GIỜ RA VÀ TIỀN
            var trans = _db.Transactions.FirstOrDefault(t => t.IDDoXe == idDoXe && t.ThoiGianRa == null);
            if (trans != null)
            {
                trans.ThoiGianRa = DateTime.Now;
                trans.ThanhTien = tien;
            }
            else
            {

                _db.Transactions.Add(new Transaction
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

            // Reset slot về trống
            slot.TrangThai = "Trống";
            slot.TenKhachHang = null;
            slot.SoDienThoai = null;
            slot.BienSoXe = null;
            slot.MaLoaiXe = null;
            slot.HinhAnhVao = null;
            slot.IDNguoiDung = null;
            slot.ThoiGianVao = null;

            _db.SaveChanges();

            return tien;
        }

        // ===== QUẢN LÝ USER =====
        public List<User> GetAllUsers() => _db.Users.ToList();

        public User GetUserByID(string id) => _db.Users.FirstOrDefault(u => u.ID == id);

        public bool AddUser(User user)
        {
            if (_db.Users.Any(u => u.ID == user.ID)) return false;
            _db.Users.Add(user);
            _db.SaveChanges();
            return true;
        }

        public void RemoveUser(string id)
        {
            var u = _db.Users.FirstOrDefault(x => x.ID == id);
            if (u != null)
            {
                _db.Users.Remove(u);
                _db.SaveChanges();
            }
        }

        public void UpdateUser(User updated)
        {
            var u = _db.Users.FirstOrDefault(x => x.ID == updated.ID);
            if (u == null) return;
            u.HoTen = updated.HoTen;
            u.MatKhau = updated.MatKhau;
            u.SDT = updated.SDT;
            u.BienSo = updated.BienSo;
            u.VaiTro = updated.VaiTro;
            _db.SaveChanges();
        }

        public bool DoiMatKhau(string id, string matKhauCu, string matKhauMoi)
        {
            var u = _db.Users.FirstOrDefault(x => x.ID == id && x.MatKhau == matKhauCu);
            if (u == null) return false;
            u.MatKhau = matKhauMoi;
            _db.SaveChanges();
            return true;
        }

        // ===== BÁO CÁO =====
        public List<Transaction> GetTransactions() => _db.Transactions.ToList();

        public decimal GetTotalRevenue() => _db.Transactions.Any() ? _db.Transactions.Sum(t => t.ThanhTien) : 0;

        public List<Transaction> GetTransactionsByDate(DateTime tuNgay, DateTime denNgay)
        {
            var denNgayCuoi = denNgay.Date.AddDays(1).AddTicks(-1);
            return _db.Transactions.Where(t => t.ThoiGianVao >= tuNgay.Date && t.ThoiGianVao <= denNgayCuoi && t.ThoiGianRa != null).ToList();
        }

        public decimal TinhPhiDuTinh(ParkingSlot slot)
        {
            if (!slot.ThoiGianVao.HasValue) return 0;
            var loaiXe = _db.LoaiXes.FirstOrDefault(l => l.MaLoaiXe == slot.MaLoaiXe);
            decimal phiGio = loaiXe != null ? (decimal)loaiXe.PhiMoiGio : 10000m;
            double phut = (DateTime.Now - slot.ThoiGianVao.Value).TotalMinutes;
            double gio = phut <= 60 ? 1 : Math.Ceiling(phut / 60.0);
            return (decimal)gio * phiGio;
        }
    }
}