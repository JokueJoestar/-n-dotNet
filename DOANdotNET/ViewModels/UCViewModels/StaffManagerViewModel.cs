using DOANdotNET.Models;
using DOANdotNET.ViewModels.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DOANdotNET.ViewModels.Services;
using System;
namespace DOANdotNET.ViewModels.UCViewModels
{
    public class StaffManagerViewModel : BaseViewModel
    {
        private readonly ParkingService _service; 

        // ===== 1. DANH SÁCH BẢNG =====
        private ObservableCollection<User> _listStaff;
        public ObservableCollection<User> ListStaff
        {
            get => _listStaff;
            set => SetProperty(ref _listStaff, value);
        }

        private User _selectedStaff;
        public User SelectedStaff
        {
            get => _selectedStaff;
            set => SetProperty(ref _selectedStaff, value);
        }

        // ===== 2. THÔNG TIN TRÊN FORM =====
        private string _staffID;
        public string StaffID { get => _staffID; set => SetProperty(ref _staffID, value); }

        private string _staffName;
        public string StaffName { get => _staffName; set => SetProperty(ref _staffName, value); }

        private string _staffSDT;
        public string StaffSDT { get => _staffSDT; set => SetProperty(ref _staffSDT, value); }

        private string _staffBienSo;
        public string StaffBienSo { get => _staffBienSo; set => SetProperty(ref _staffBienSo, value); }

        private string _staffMatKhau;
        public string StaffMatKhau { get => _staffMatKhau; set => SetProperty(ref _staffMatKhau, value); }

        private string _staffRole = "nhanvien";
        public string StaffRole { get => _staffRole; set => SetProperty(ref _staffRole, value); }

        private bool _isShowPassword = false;
        public bool IsShowPassword { get => _isShowPassword; set => SetProperty(ref _isShowPassword, value); }

        private bool _isEditMode = false;

        // ===== 3. COMMANDS =====
        public ICommand TogglePasswordCommand { get; set; }
        public ICommand AddStaffCommand { get; set; }
        public ICommand UpdateStaffCommand { get; set; }
        public ICommand ClearStaffCommand { get; set; }
        public ICommand FillStaffFormCommand { get; set; }
        public ICommand DeleteStaffCommand { get; set; }

        public StaffManagerViewModel()
        {
            _service = new ParkingService(); // Khởi tạo Service

            // Lấy dữ liệu thật từ SQL Server
            LoadData();

            // LOGIC: ẨN / HIỆN MẬT KHẨU (Giữ nguyên)
            TogglePasswordCommand = new RelayCommand(p =>
            {
                if (p is PasswordBox pbox)
                {
                    if (IsShowPassword)
                        StaffMatKhau = pbox.Password;
                    else
                        pbox.Password = StaffMatKhau;
                }
            }, _ => true);

            // LOGIC: THÊM MỚI TÀI KHOẢN (Đã nối DB)
            AddStaffCommand = new RelayCommand(p =>
            {
                string pass = IsShowPassword ? StaffMatKhau : (p as PasswordBox)?.Password;

                if (string.IsNullOrWhiteSpace(StaffID) || string.IsNullOrWhiteSpace(StaffName))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ ID và Họ tên!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StaffRole != "khachhang" && string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Nhân viên và Admin phải có mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Đưa đối tượng xuống Service để lưu
                var newUser = new User
                {
                    ID = StaffID,
                    HoTen = StaffName,
                    SDT = StaffSDT ?? "",
                    BienSo = StaffBienSo ?? "",
                    MatKhau = pass ?? "",
                    VaiTro = StaffRole,
                    NgayTao = DateTime.Now 
                };

                bool ok = _service.AddUser(newUser);

                if (ok)
                {
                    MessageBox.Show("Thêm mới tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData(); 
                    ClearForm(p as PasswordBox);
                }
                else
                {
                    MessageBox.Show("ID / Tên đăng nhập này đã tồn tại trong CSDL!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }, p => !_isEditMode);

            // LOGIC: CẬP NHẬT TÀI KHOẢN (Đã nối DB, code ngắn gọn hơn nhiều)
            UpdateStaffCommand = new RelayCommand(p =>
            {
                if (SelectedStaff == null) return;

                string pass = IsShowPassword ? StaffMatKhau : (p as PasswordBox)?.Password;
                if (string.IsNullOrWhiteSpace(StaffName))
                {
                    MessageBox.Show("Họ tên không được để trống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StaffRole != "khachhang" && string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Nhân viên và Admin phải có mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Chỉ cần gọi Service Update và Refresh lại bảng
                var updatedUser = new User
                {
                    ID = SelectedStaff.ID, 
                    HoTen = StaffName,
                    VaiTro = StaffRole,
                    MatKhau = pass ?? "",
                    SDT = StaffSDT ?? "",
                    BienSo = StaffBienSo ?? ""
                };

                _service.UpdateUser(updatedUser);

                MessageBox.Show("Cập nhật tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData(); 
                ClearForm(p as PasswordBox);

            }, p => _isEditMode);

            // LOGIC: LÀM TRỐNG FORM
            ClearStaffCommand = new RelayCommand(p => ClearForm(null), _ => true);

            // LOGIC: ĐƯA DỮ LIỆU TỪ BẢNG LÊN FORM ĐỂ SỬA
            FillStaffFormCommand = new RelayCommand(p =>
            {
                if (p is User u)
                {
                    SelectedStaff = u;
                    StaffID = u.ID;
                    StaffName = u.HoTen;
                    StaffSDT = u.SDT;
                    StaffBienSo = u.BienSo;
                    StaffRole = u.VaiTro;
                    StaffMatKhau = u.MatKhau;

                    _isEditMode = true;
                    IsShowPassword = true; // Hiện pass ra ngoài

                    CommandManager.InvalidateRequerySuggested();
                }
            }, _ => true);

            // LOGIC: XÓA TÀI KHOẢN (Đã nối DB)
            DeleteStaffCommand = new RelayCommand(p =>
            {
                if (p is User u)
                {
                    if (u.ID.ToLower() == "admin")
                    {
                        MessageBox.Show("Không thể xóa tài khoản Admin gốc!", "Từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (MessageBox.Show($"Bạn có chắc muốn xóa tài khoản '{u.HoTen}' khỏi hệ thống?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        _service.RemoveUser(u.ID); // Xóa khỏi DB
                        LoadData(); // Cập nhật lại UI

                        if (_isEditMode && SelectedStaff == u) ClearForm(null);
                    }
                }
            }, _ => true);
        }

        //  Rút dữ liệu người dùng từ DB
        private void LoadData()
        {
            var dbUsers = _service.GetAllUsers();
            ListStaff = new ObservableCollection<User>(dbUsers);
        }

        private void ClearForm(PasswordBox pbox)
        {
            StaffID = "";
            StaffName = "";
            StaffSDT = "";
            StaffBienSo = "";
            StaffMatKhau = "";
            StaffRole = "nhanvien";

            if (pbox != null) pbox.Password = "";
            IsShowPassword = false;

            SelectedStaff = null;
            _isEditMode = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}