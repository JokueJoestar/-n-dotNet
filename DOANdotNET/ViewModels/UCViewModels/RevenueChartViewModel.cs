using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DOANdotNET.Models;
using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;

namespace DOANdotNET.ViewModels.UCViewModels
{
    public class ChartItem
    {
        public string Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public double BarWidth { get; set; }
    }

    public class GiaoDichRow
    {
        public string MaGiaoDich { get; set; }
        public string IDDoXe { get; set; }
        public string TenKhachHang { get; set; }
        public string BienSoXe { get; set; }
        public DateTime? ThoiGianVao { get; set; }
        public string ThoiGianRaHienThi { get; set; }
        public string TrangThai { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class LoaiXeFilter
    {
        public string MaLoaiXe { get; set; }
        public string TenLoaiXe { get; set; }
    }

    public class RevenueChartViewModel : BaseViewModel
    {
        private readonly ParkingService _service;
        private const double MAX_BAR_WIDTH = 340.0;

        // ===== BỘ LỌC =====
        private DateTime _tuNgay;
        public DateTime TuNgay
        {
            get => _tuNgay;
            set => SetProperty(ref _tuNgay, value);
        }

        private DateTime _denNgay;
        public DateTime DenNgay
        {
            get => _denNgay;
            set => SetProperty(ref _denNgay, value);
        }

        private List<LoaiXeFilter> _danhSachLoaiXe;
        public List<LoaiXeFilter> DanhSachLoaiXe
        {
            get => _danhSachLoaiXe;
            set => SetProperty(ref _danhSachLoaiXe, value);
        }

        private LoaiXeFilter _loaiXeChon;
        public LoaiXeFilter LoaiXeChon
        {
            get => _loaiXeChon;
            set => SetProperty(ref _loaiXeChon, value);
        }

        // ===== THẺ TỔNG QUAN =====
        private int _tongGiaoDich;
        public int TongGiaoDich
        {
            get => _tongGiaoDich;
            set => SetProperty(ref _tongGiaoDich, value);
        }

        private int _daThanhToan;
        public int DaThanhToan
        {
            get => _daThanhToan;
            set => SetProperty(ref _daThanhToan, value);
        }

        private int _xeTrongBai;
        public int XeTrongBai
        {
            get => _xeTrongBai;
            set => SetProperty(ref _xeTrongBai, value);
        }

        // ===== BIỂU ĐỒ =====
        private ObservableCollection<ChartItem> _chartItems;
        public ObservableCollection<ChartItem> ChartItems
        {
            get => _chartItems;
            set => SetProperty(ref _chartItems, value);
        }

        private bool _coChartData;
        public bool CoChartData
        {
            get => _coChartData;
            set => SetProperty(ref _coChartData, value);
        }

        private decimal _tongDoanhThu;
        public decimal TongDoanhThu
        {
            get => _tongDoanhThu;
            set => SetProperty(ref _tongDoanhThu, value);
        }

        private int _tongLuotXe;
        public int TongLuotXe
        {
            get => _tongLuotXe;
            set => SetProperty(ref _tongLuotXe, value);
        }

        private decimal _trungBinhNgay;
        public decimal TrungBinhNgay
        {
            get => _trungBinhNgay;
            set => SetProperty(ref _trungBinhNgay, value);
        }

        // ===== BẢNG GIAO DỊCH =====
        private ObservableCollection<GiaoDichRow> _danhSachGiaoDich;
        public ObservableCollection<GiaoDichRow> DanhSachGiaoDich
        {
            get => _danhSachGiaoDich;
            set => SetProperty(ref _danhSachGiaoDich, value);
        }

        private int _soGiaoDichHienThi;
        public int SoGiaoDichHienThi
        {
            get => _soGiaoDichHienThi;
            set => SetProperty(ref _soGiaoDichHienThi, value);
        }

        private string _footerLeft;
        public string FooterLeft
        {
            get => _footerLeft;
            set => SetProperty(ref _footerLeft, value);
        }

        // ===== COMMAND =====
        public ICommand LocCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand XuatExcelCommand { get; }

        // ===== KHỞI TẠO =====
        public RevenueChartViewModel()
        {
            _service = new ParkingService();
            _tuNgay = DateTime.Today;
            _denNgay = DateTime.Today;
            ChartItems = new ObservableCollection<ChartItem>();
            DanhSachGiaoDich = new ObservableCollection<GiaoDichRow>();

            var loaiXes = _service.GetAllLoaiXe();
            DanhSachLoaiXe = new List<LoaiXeFilter>
            {
                new LoaiXeFilter { MaLoaiXe = null, TenLoaiXe = "Tất cả" }
            };
            DanhSachLoaiXe.AddRange(loaiXes.Select(l => new LoaiXeFilter
            {
                MaLoaiXe = l.MaLoaiXe,
                TenLoaiXe = l.TenLoaiXe
            }));
            _loaiXeChon = DanhSachLoaiXe[0];

            LocCommand = new RelayCommand(_ => LoadData());
            RefreshCommand = new RelayCommand(_ =>
            {
                TuNgay = DateTime.Today;
                DenNgay = DateTime.Today;
                LoaiXeChon = DanhSachLoaiXe[0];
                LoadData();
            });
            XuatExcelCommand = new RelayCommand(_ => XuatExcel());

            LoadData();
        }

        // ===== TẢI DỮ LIỆU =====
        private void LoadData()
        {
            var all = _service.GetTransactionsByDate(TuNgay, DenNgay);

            var filtered = (LoaiXeChon == null || LoaiXeChon.MaLoaiXe == null)
                ? all
                : all.Where(t => t.MaLoaiXe == LoaiXeChon.MaLoaiXe).ToList();

            // Thẻ tổng quan
            TongGiaoDich = all.Count;
            DaThanhToan = all.Count(t => t.ThoiGianRa != null);
            XeTrongBai = _service.CountCoXe();
            TongDoanhThu = filtered.Sum(t => t.ThanhTien);
            TongLuotXe = filtered.Count;

            // Biểu đồ
            var grouped = filtered
                .Where(t => t.ThoiGianRa != null)
                .GroupBy(t => t.ThoiGianRa.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Ngay = g.Key, DoanhThu = g.Sum(t => t.ThanhTien) })
                .ToList();

            decimal maxDT = grouped.Count > 0 ? grouped.Max(g => g.DoanhThu) : 1;
            int soNgay = grouped.Count;
            TrungBinhNgay = soNgay > 0 ? TongDoanhThu / soNgay : 0;

            ChartItems.Clear();
            foreach (var g in grouped)
            {
                ChartItems.Add(new ChartItem
                {
                    Ngay = g.Ngay.ToString("dd/MM"),
                    DoanhThu = g.DoanhThu,
                    BarWidth = maxDT > 0
                        ? Math.Max((double)(g.DoanhThu / maxDT) * MAX_BAR_WIDTH, 4)
                        : 4
                });
            }

            CoChartData = ChartItems.Count > 0;  // ← nằm TRONG class, đúng rồi

            // Bảng giao dịch
            DanhSachGiaoDich.Clear();
            foreach (var t in filtered.OrderByDescending(t => t.ThoiGianVao))
            {
                DanhSachGiaoDich.Add(new GiaoDichRow
                {
                    MaGiaoDich = t.MaGiaoDich,
                    IDDoXe = t.IDDoXe,
                    TenKhachHang = t.TenKhachHang,
                    BienSoXe = t.BienSoXe,
                    ThoiGianVao = t.ThoiGianVao,
                    ThoiGianRaHienThi = t.ThoiGianRa.HasValue
                        ? t.ThoiGianRa.Value.ToString("dd/MM HH:mm")
                        : "Đang gửi",
                    TrangThai = t.ThoiGianRa.HasValue ? "Đã thanh toán" : "Đang gửi",
                    ThanhTien = t.ThanhTien
                });
            }

            SoGiaoDichHienThi = DanhSachGiaoDich.Count;
            FooterLeft = $"Tổng cộng {DaThanhToan} giao dịch đã thanh toán";
        }

        // ===== XUẤT CSV RA DESKTOP =====
        private void XuatExcel()
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"DoanhThu_{TuNgay:ddMMyyyy}_{DenNgay:ddMMyyyy}.csv");

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Mã GD,Vị trí,Khách hàng,Biển số,Giờ vào,Giờ ra,Trạng thái,Thành tiền");
                foreach (var r in DanhSachGiaoDich)
                {
                    sb.AppendLine($"{r.MaGiaoDich},{r.IDDoXe},{r.TenKhachHang},{r.BienSoXe}," +
                                  $"{r.ThoiGianVao:dd/MM/yyyy HH:mm},{r.ThoiGianRaHienThi}," +
                                  $"{r.TrangThai},{r.ThanhTien:N0}");
                }
                System.IO.File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);
                System.Windows.MessageBox.Show($"Đã xuất file ra Desktop:\n{path}",
                    "Xuất thành công", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }  // ← đóng RevenueChartViewModel
}      // ← đóng namespace