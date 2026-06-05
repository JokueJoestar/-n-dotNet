using System;

namespace DOANdotNET.Models
{
    public class DatTruoc
    {
        public string MaDatTruoc { get; set; }
        public string IDKhachHang { get; set; }
        public string KhuVuc { get; set; }
        public string MaLoaiXe { get; set; }
        public DateTime ThoiGianDen { get; set; }
        public DateTime ThoiGianHetHan { get; set; }
        public string TrangThai { get; set; }
        public string IDDoXe { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal? GiaDoXe { get; set; }

        // Navigation (load thủ công trong service)
        public virtual LoaiXe LoaiXe { get; set; }
        public virtual User User { get; set; }

        // ── Computed properties cho UI ──────────────────────────────
        public string ThoiGianDenHienThi
        {
            get => ThoiGianDen.ToString("dd/MM/yyyy  HH:mm");
            set { } // cho WPF binding không lỗi
        }

        public string GiaHienThi
        {
            get => GiaDoXe.HasValue ? GiaDoXe.Value.ToString("N0") + " đ/giờ" : "—";
            set { } // cho WPF binding không lỗi
        }

        public System.Windows.Media.Brush TrangThaiMau
        {
            get
            {
                switch (TrangThai)
                {
                    case "Đã xác nhận": return System.Windows.Media.Brushes.LimeGreen;
                    case "Chờ xử lý": return System.Windows.Media.Brushes.Orange;
                    case "Đã hủy": return System.Windows.Media.Brushes.Crimson;
                    case "Hoàn thành": return System.Windows.Media.Brushes.Gray;
                    default: return System.Windows.Media.Brushes.LightGray;
                }
            }
        }

        public bool CoTheHuy =>
            TrangThai == "Chờ xử lý" || TrangThai == "Đã xác nhận";
    }
}