using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace DOANdotNET.ViewModels
{
    public class CheckOutViewModel : BaseViewModel
    {
        private readonly ParkingService _service;
        private readonly string _idSlot;

        // ===== THÔNG TIN HIỂN THỊ =====
        private string _resultBienSo;
        public string ResultBienSo
        {
            get => _resultBienSo;
            set => SetProperty(ref _resultBienSo, value);
        }

        private string _resultIDDoXe;
        public string ResultIDDoXe
        {
            get => _resultIDDoXe;
            set => SetProperty(ref _resultIDDoXe, value);
        }

        private string _resultTenKhach;
        public string ResultTenKhach
        {
            get => _resultTenKhach;
            set => SetProperty(ref _resultTenKhach, value);
        }

        private DateTime _resultGioVao;
        public DateTime ResultGioVao
        {
            get => _resultGioVao;
            set
            {
                SetProperty(ref _resultGioVao, value);
                OnPropertyChanged(nameof(TongThoiGian));
            }
        }

        public DateTime ResultGioRa { get; } = DateTime.Now;

        // Tính tổng thời gian dạng "X giờ Y phút"
        public string TongThoiGian
        {
            get
            {
                var span = ResultGioRa - ResultGioVao;
                int gio = (int)span.TotalHours;
                int phut = span.Minutes;
                if (gio == 0) return $"{phut} phút";
                if (phut == 0) return $"{gio} giờ";
                return $"{gio} giờ {phut} phút";
            }
        }

        private decimal _thanhTien;
        public decimal ThanhTien
        {
            get => _thanhTien;
            set => SetProperty(ref _thanhTien, value);
        }

        // ===== ẢNH ĐỐI CHIẾU =====
        private BitmapImage _anhXeVao;
        public BitmapImage AnhXeVao
        {
            get => _anhXeVao;
            set => SetProperty(ref _anhXeVao, value);
        }

        private BitmapImage _anhXeRa;
        public BitmapImage AnhXeRa
        {
            get => _anhXeRa;
            set
            {
                SetProperty(ref _anhXeRa, value);
                OnPropertyChanged(nameof(HasAnhXeRa)); // Cập nhật lại UI nút bấm
            }
        }

        // Kiểm tra đã có ảnh xe ra hay chưa (Trả về true nếu có ảnh)
        public bool HasAnhXeRa => AnhXeRa != null;


        // ===== ERROR =====
        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                SetProperty(ref _errorMsg, value);
                OnPropertyChanged(nameof(HasError));
            }
        }
        public bool HasError => !string.IsNullOrEmpty(ErrorMsg);

        // ===== COMMAND =====
        public ICommand ConfirmCommand { get; }
        public ICommand CaptureCameraCommand { get; } 

        // ===== CONSTRUCTOR =====
        public CheckOutViewModel(string idSlot)
        {
            _idSlot = idSlot;
            _service = new ParkingService();

            LoadSlotInfo();

            ConfirmCommand = new RelayCommand(p => Confirm(p as Window));

            // Lệnh giả lập Camera (Chọn file từ máy tính)
            CaptureCameraCommand = new RelayCommand(p =>
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Title = "Giả lập Camera: Chọn ảnh xe ra";
                dlg.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (dlg.ShowDialog() == true)
                {
                    AnhXeRa = LoadImageAbsolute(dlg.FileName);
                }
            });
        }

        // ===== LOAD THÔNG TIN =====
        private void LoadSlotInfo()
        {
            var slots = _service.GetParkingSlots();
            var slot = slots.Find(s => s.IDDoXe == _idSlot);
            if (slot == null) return;

            ResultIDDoXe = slot.IDDoXe;
            ResultBienSo = slot.BienSoXe;
            ResultTenKhach = slot.TenKhachHang;
            ResultGioVao = slot.ThoiGianVao ?? DateTime.Now;

            // Tính tiền dự tính
            ThanhTien = _service.TinhPhiDuTinh(slot);

            // Load ảnh xe vào nếu có
            if (!string.IsNullOrEmpty(slot.HinhAnhVao))
                AnhXeVao = LoadImage(slot.HinhAnhVao);
        }

        // ===== XÁC NHẬN THANH TOÁN =====
        private void Confirm(Window window)
        {
            var result = MessageBox.Show(
                $"Xác nhận thanh toán và cho xe ra?\n\n" +
                $"Biển số : {ResultBienSo}\n" +
                $"Vị trí  : {ResultIDDoXe}\n" +
                $"Số tiền : {ThanhTien:N0} đ",
                "Xác nhận thanh toán",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            
            decimal tienThuc = _service.CheckOut(_idSlot, out string tenKhach);

            MessageBox.Show(
                $"✅ Thanh toán thành công!\n\n" +
                $"Khách hàng : {tenKhach}\n" +
                $"Biển số    : {ResultBienSo}\n" +
                $"Tổng tiền : {tienThuc:N0} đ\n" +
                $"Giờ ra    : {DateTime.Now:dd/MM/yyyy HH:mm}",
                "Thanh toán thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        // ===== HELPER: Load ảnh từ path Tương đối (Relative string) =====
        private BitmapImage LoadImage(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePath);
                if (!File.Exists(fullPath)) return null;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                return bmp;
            }
            catch { return null; }
        }

        // ===== HELPER: Load ảnh từ path Tuyệt đối (Tuyệt đối từ OpenFileDialog) =====
        private BitmapImage LoadImageAbsolute(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath)) return null;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad; // Tránh bị lock file khi load xong
                bmp.EndInit();
                return bmp;
            }
            catch { return null; }
        }
    }
}