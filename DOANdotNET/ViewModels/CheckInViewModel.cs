using DOANdotNET.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using DOANdotNET.Models;
using DOANdotNET.ViewModels.Services;
using System.Collections.ObjectModel;
using System.IO;
namespace DOANdotNET.ViewModels
{
    public class CheckInViewModel : BaseViewModel
    {
        private readonly ParkingService _service;
        private readonly string _idSlot;

        // ===== PROPERTIES =====
        private string _tenKhachHang;
        public string TenKhachHang
        {
            get => _tenKhachHang;
            set => SetProperty(ref _tenKhachHang, value);
        }

        private string _soDienThoai;
        public string SoDienThoai
        {
            get => _soDienThoai;
            set => SetProperty(ref _soDienThoai, value);
        }

        private string _bienSoXe;
        public string BienSoXe
        {
            get => _bienSoXe;
            set => SetProperty(ref _bienSoXe, value?.ToUpper());
        }

        private LoaiXe _selectedLoaiXe;
        public LoaiXe SelectedLoaiXe
        {
            get => _selectedLoaiXe;
            set => SetProperty(ref _selectedLoaiXe, value);
        }

        public ObservableCollection<LoaiXe> ListLoaiXe { get; }
            = new ObservableCollection<LoaiXe>();

        public DateTime GioVao { get; } = DateTime.Now;

        // ===== ẢNH =====
        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                SetProperty(ref _imagePath, value);
                OnPropertyChanged(nameof(ChuaCoAnh));
            }
        }

        public bool ChuaCoAnh => string.IsNullOrEmpty(ImagePath);

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

        // ===== COMMANDS =====
        public ICommand UploadImageCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        // ===== CONSTRUCTOR =====
        public CheckInViewModel(string idSlot)
        {
            _idSlot = idSlot;
            _service = new ParkingService();

            // Load danh sách loại xe
            foreach (var lx in _service.GetAllLoaiXe())
                ListLoaiXe.Add(lx);
            if (ListLoaiXe.Count > 0)
                SelectedLoaiXe = ListLoaiXe[0];

            UploadImageCommand = new RelayCommand(_ => UploadImage());
            ConfirmCommand = new RelayCommand(p => Confirm(p as Window));
            CancelCommand = new RelayCommand(p =>
            {
                if (p is Window w) { w.DialogResult = false; w.Close(); }
            });
        }

        // ===== UPLOAD ẢNH =====
        private void UploadImage()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh xe",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string ext = Path.GetExtension(dialog.FileName);
                string fileName = $"checkin_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                string destPath = Path.Combine(folder, fileName);

                File.Copy(dialog.FileName, destPath, overwrite: true);

                // Gán đường dẫn TUYỆT ĐỐI để giao diện hiển thị ngay lập tức
                ImagePath = destPath;
                ErrorMsg = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMsg = "Lỗi tải ảnh: " + ex.Message;
            }
        }

        // ===== XÁC NHẬN =====
        private void Confirm(Window window)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TenKhachHang))
            { ErrorMsg = "Vui lòng nhập tên khách hàng!"; return; }

            if (string.IsNullOrWhiteSpace(BienSoXe))
            { ErrorMsg = "Vui lòng nhập biển số xe!"; return; }

            if (SelectedLoaiXe == null)
            { ErrorMsg = "Vui lòng chọn loại xe!"; return; }

            // Xử lý lại đường dẫn ảnh: Chỉ lấy tên file ghép với "Images/" để lưu DB
            string relativeImagePath = string.IsNullOrEmpty(ImagePath)
                ? string.Empty
                : $"Images/{Path.GetFileName(ImagePath)}";

            // Gọi Service
            bool ok = _service.CheckIn(
                idDoXe: _idSlot,
                tenKhach: TenKhachHang.Trim(),
                sdt: SoDienThoai?.Trim(),
                bienSo: BienSoXe.Trim(),
                maLoaiXe: SelectedLoaiXe.MaLoaiXe,
                hinhAnh: relativeImagePath 
            );

            if (!ok)
            {
                ErrorMsg = "Vị trí đã có xe hoặc không tồn tại!";
                return;
            }

            MessageBox.Show(
                $"✅ Xe {BienSoXe} đã vào bãi!\n" +
                $"Vị trí: {_idSlot}\n" +
                $"Giờ vào: {GioVao:dd/MM/yyyy HH:mm}",
                "Check-in thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
