using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DOANdotNET.Models;
using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;

namespace DOANdotNET.ViewModels.UCViewModels
{
    public class VehicleTypeViewModel : BaseViewModel
    {
        private readonly ParkingService _service; 

        // ===== DANH SÁCH & TRẠNG THÁI CHỌN =====
        public ObservableCollection<LoaiXe> ListLoaiXe { get; set; } = new ObservableCollection<LoaiXe>();

        private LoaiXe _selectedLoaiXe;
        public LoaiXe SelectedLoaiXe
        {
            get => _selectedLoaiXe;
            set => SetProperty(ref _selectedLoaiXe, value);
        }

        // ===== CÁC TRƯỜNG TRÊN FORM =====
        private string _maLoaiXe;
        public string MaLoaiXe { get => _maLoaiXe; set => SetProperty(ref _maLoaiXe, value); }

        private string _tenLoaiXe;
        public string TenLoaiXe { get => _tenLoaiXe; set => SetProperty(ref _tenLoaiXe, value); }

        private string _phiMoiGio;
        public string PhiMoiGio { get => _phiMoiGio; set => SetProperty(ref _phiMoiGio, value); }

        private string _formTitle = "➕ Thêm loại xe mới";
        public string FormTitle { get => _formTitle; set => SetProperty(ref _formTitle, value); }

        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set { SetProperty(ref _errorMsg, value); OnPropertyChanged(nameof(HasError)); }
        }
        public bool HasError => !string.IsNullOrEmpty(ErrorMsg);

        private bool _isEditMode = false;

        // ===== COMMANDS =====
        public ICommand ThemMoiCommand { get; }
        public ICommand CapNhatCommand { get; }
        public ICommand LamTrongCommand { get; }
        public ICommand SuaCommand { get; }
        public ICommand XoaCommand { get; }

        public VehicleTypeViewModel()
        {
            _service = new ParkingService(); 

            
            LoadData();

            ThemMoiCommand = new RelayCommand(_ => ThemMoi(), _ => !_isEditMode);
            CapNhatCommand = new RelayCommand(_ => CapNhat(), _ => _isEditMode);
            LamTrongCommand = new RelayCommand(_ => LamTrong(), _ => true);
            SuaCommand = new RelayCommand(o => Sua(o as LoaiXe), _ => true);
            XoaCommand = new RelayCommand(o => Xoa(o as LoaiXe), _ => true);
        }

        // Kéo danh sách loại xe từ DB và tự động đếm số xe đang đỗ
        private void LoadData()
        {
            ListLoaiXe.Clear();
            var types = _service.GetAllLoaiXe();

            
            var activeSlots = _service.GetParkingSlots().Where(s => s.TrangThai == "Đang gửi").ToList();

            foreach (var lx in types)
            {
                
                lx.SoXeDangGui = activeSlots.Count(s => s.MaLoaiXe == lx.MaLoaiXe);
                ListLoaiXe.Add(lx);
            }
        }

        // --- LOGIC XỬ LÝ CHÍNH ---
        private void ThemMoi()
        {
            if (!Validate()) return;

            if (ListLoaiXe.Any(x => x.MaLoaiXe.ToUpper() == MaLoaiXe.ToUpper()))
            {
                ErrorMsg = "Mã loại xe này đã tồn tại!";
                return;
            }

            var newLoaiXe = new LoaiXe
            {
                MaLoaiXe = MaLoaiXe.ToUpper(),
                TenLoaiXe = TenLoaiXe,
                PhiMoiGio = decimal.Parse(PhiMoiGio)
            };

            _service.AddLoaiXe(newLoaiXe); // Ghi xuống SQL Server

            MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
            LamTrong();
        }

        private void CapNhat()
        {
            if (SelectedLoaiXe == null) { ErrorMsg = "Vui lòng chọn loại xe cần cập nhật!"; return; }
            if (!Validate()) return;

            SelectedLoaiXe.TenLoaiXe = TenLoaiXe;
            SelectedLoaiXe.PhiMoiGio = decimal.Parse(PhiMoiGio);

            _service.UpdateLoaiXe(SelectedLoaiXe); // Báo SQL Server lưu

            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
            LamTrong();
        }

        private void Sua(LoaiXe item)
        {
            if (item == null) return;

            SelectedLoaiXe = item;
            MaLoaiXe = item.MaLoaiXe;
            TenLoaiXe = item.TenLoaiXe;
            PhiMoiGio = item.PhiMoiGio.ToString("G29");

            _isEditMode = true;
            FormTitle = "✏️ Cập nhật loại xe: " + item.MaLoaiXe;
            ErrorMsg = string.Empty;

            CommandManager.InvalidateRequerySuggested();
        }

        private void Xoa(LoaiXe item)
        {
            if (item == null) return;

            if (item.SoXeDangGui > 0)
            {
                MessageBox.Show("Không thể xóa loại xe đang có xe gửi trong bãi!", "Từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Cậu có chắc muốn xóa '{item.TenLoaiXe}' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _service.RemoveLoaiXe(item.MaLoaiXe); // Xóa thẳng dưới SQL Server
                LoadData();
                if (_isEditMode && SelectedLoaiXe?.MaLoaiXe == item.MaLoaiXe) LamTrong();
            }
        }

        private void LamTrong()
        {
            MaLoaiXe = string.Empty;
            TenLoaiXe = string.Empty;
            PhiMoiGio = string.Empty;
            SelectedLoaiXe = null;
            ErrorMsg = string.Empty;

            _isEditMode = false;
            FormTitle = "➕ Thêm loại xe mới";

            CommandManager.InvalidateRequerySuggested();
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(MaLoaiXe))
            { ErrorMsg = "Vui lòng nhập mã loại xe!"; return false; }

            if (string.IsNullOrWhiteSpace(TenLoaiXe))
            { ErrorMsg = "Vui lòng nhập tên loại xe!"; return false; }

            if (!decimal.TryParse(PhiMoiGio, out _) || decimal.Parse(PhiMoiGio) < 0)
            { ErrorMsg = "Phí/giờ phải là số dương hợp lệ!"; return false; }

            ErrorMsg = string.Empty;
            return true;
        }
    }
}