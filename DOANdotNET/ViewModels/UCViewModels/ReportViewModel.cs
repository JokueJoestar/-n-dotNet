using DOANdotNET.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DOANdotNET.ViewModels.Services;
namespace DOANdotNET.ViewModels.UCViewModels
{
    public class ReportModel : BaseViewModel
    {
        public string MaGiaoDich { get; set; }
        public string IDDoXe { get; set; }
        public string TenKhachHang { get; set; }
        public string BienSoXe { get; set; }
        public DateTime? ThoiGianVao { get; set; }
        public DateTime? ThoiGianRa { get; set; }
        public string TrangThai { get; set; }
        public decimal ThanhTien { get; set; }
    }

    // CLASS CHÍNH: VIEWMODEL
    public class ReportViewModel : BaseViewModel
    {
        private readonly ParkingService _service; 

        private List<ReportModel> _allReports;

        private ObservableCollection<ReportModel> _listReport;
        public ObservableCollection<ReportModel> ListReport
        {
            get => _listReport;
            set => SetProperty(ref _listReport, value);
        }

        public ObservableCollection<string> ListTrangThai { get; set; }

        private int _tongGiaoDich;
        public int TongGiaoDich { get => _tongGiaoDich; set => SetProperty(ref _tongGiaoDich, value); }

        private int _daThanhToan;
        public int DaThanhToan { get => _daThanhToan; set => SetProperty(ref _daThanhToan, value); }

        private decimal _totalRevenue;
        public decimal TotalRevenue { get => _totalRevenue; set => SetProperty(ref _totalRevenue, value); }

        private int _xeDangTrongBai;
        public int XeDangTrongBai { get => _xeDangTrongBai; set => SetProperty(ref _xeDangTrongBai, value); }

        private DateTime? _tuNgay;
        public DateTime? TuNgay { get => _tuNgay; set => SetProperty(ref _tuNgay, value); }

        private DateTime? _denNgay;
        public DateTime? DenNgay { get => _denNgay; set => SetProperty(ref _denNgay, value); }

        private string _selectedTrangThai;
        public string SelectedTrangThai { get => _selectedTrangThai; set => SetProperty(ref _selectedTrangThai, value); }

        public ICommand FilterCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public ReportViewModel()
        {
            _service = new ParkingService(); // Khởi tạo Service

            ListTrangThai = new ObservableCollection<string> { "Tất cả", "Đang gửi", "Đã xong" };
            SelectedTrangThai = "Tất cả";
            TuNgay = DateTime.Now.Date;
            DenNgay = DateTime.Now.Date;

            LoadDataFromDB();

            FilterCommand = new RelayCommand(p =>
            {
                ApplyFilter();
            });

            RefreshCommand = new RelayCommand(p =>
            {
                TuNgay = DateTime.Now.Date;
                DenNgay = DateTime.Now.Date;
                SelectedTrangThai = "Tất cả";
                LoadDataFromDB(); 
            });

            ExportExcelCommand = new RelayCommand(p => MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information));
            PrintCommand = new RelayCommand(p => MessageBox.Show("Đang kết nối máy in...", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void LoadDataFromDB()
        {
            var dbTransactions = _service.GetTransactions();

            _allReports = dbTransactions.Select(t => new ReportModel
            {
                MaGiaoDich = t.MaGiaoDich,
                IDDoXe = t.IDDoXe,
                TenKhachHang = t.TenKhachHang,
                BienSoXe = t.BienSoXe,
                ThoiGianVao = t.ThoiGianVao,
                ThoiGianRa = t.ThoiGianRa,

                
                TrangThai = t.ThoiGianRa.HasValue ? "Đã xong" : "Đang gửi",

                ThanhTien = t.ThanhTien
            }).ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allReports == null) return;

            var query = _allReports.AsEnumerable();

            if (TuNgay.HasValue)
                query = query.Where(x => x.ThoiGianVao >= TuNgay.Value.Date);
            if (DenNgay.HasValue)
                query = query.Where(x => x.ThoiGianVao <= DenNgay.Value.Date.AddDays(1).AddTicks(-1));
            if (SelectedTrangThai != "Tất cả")
                query = query.Where(x => x.TrangThai == SelectedTrangThai);

            ListReport = new ObservableCollection<ReportModel>(query);
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            TongGiaoDich = ListReport.Count;
            DaThanhToan = ListReport.Count(x => x.TrangThai == "Đã xong");
            TotalRevenue = ListReport.Sum(x => x.ThanhTien);
            XeDangTrongBai = ListReport.Count(x => x.TrangThai == "Đang gửi");
        }
    }
}
