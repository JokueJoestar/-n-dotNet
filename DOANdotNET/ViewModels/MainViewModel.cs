using System.Windows;
using System.Windows.Input;
using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;
using DOANdotNET.Views.UserControls;

namespace DOANdotNET.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set { _pageTitle = value; OnPropertyChanged(); }
        }

        public ICommand ShowParkingMapCommand { get; private set; }
        public ICommand ShowSearchCommand { get; private set; }
        public ICommand ShowStaffCommand { get; private set; }
        public ICommand ShowMyAccountCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand ShowVehicleTypeCommand { get; set; }
        public ICommand ShowRevenueChartCommand { get; private set; }
        public ICommand ShowDashboardCommand { get; private set; }
        public ICommand ShowDatTruocCommand { get; private set; }
        public ICommand ShowQuanLyDatTruocCommand { get; private set; }

        public string TenDangNhap => SessionManager.CurrentUser?.HoTen ?? "Chưa đăng nhập";
        public string VaiTroHienThi => SessionManager.CurrentUser?.VaiTroHienThi ?? "";

        public bool CanSeeReport => SessionManager.IsAdmin || SessionManager.IsNhanVien;
        public bool CanSeeStaff => SessionManager.IsAdmin;
        public bool CanSeeVehicleType => SessionManager.IsAdmin;
        public bool CanSeeMyAccount => SessionManager.CurrentUser != null;
        public bool CanSeeDatTruoc => SessionManager.IsKhachHang;
        public bool CanSeeQuanLyDatTruoc => SessionManager.IsAdmin || SessionManager.IsNhanVien;

        public MainViewModel()
        {
            if (SessionManager.IsKhachHang)
            { CurrentView = new DatTruoc(); PageTitle = "ĐẶT CHỖ TRƯỚC"; }
            else
            { CurrentView = new ParkingMap(); PageTitle = "SƠ ĐỒ BÃI ĐỖ XE"; }

            ShowParkingMapCommand = new RelayCommand(
                p => { CurrentView = new ParkingMap(); PageTitle = "SƠ ĐỒ BÃI ĐỖ XE"; },
                p => CanSeeReport);

            ShowSearchCommand = new RelayCommand(
                p => { CurrentView = new Search(); PageTitle = "TÌM KIẾM XE"; },
                p => true);

            ShowStaffCommand = new RelayCommand(
                p => { CurrentView = new StaffManager(); PageTitle = "QUẢN LÝ NHÂN SỰ"; },
                p => CanSeeStaff);

            ShowVehicleTypeCommand = new RelayCommand(
                p => { CurrentView = new VehicleTypeManager(); PageTitle = "QUẢN LÝ LOẠI XE"; },
                p => CanSeeVehicleType);

            // FIX: Xóa ShowReportCommand (đã xóa Report view)
            // Dùng RevenueChart thay thế cho tất cả chức năng báo cáo
            ShowRevenueChartCommand = new RelayCommand(
                p => { CurrentView = new RevenueChart(); PageTitle = "BIỂU ĐỒ DOANH THU"; },
                p => CanSeeReport);

            ShowDashboardCommand = new RelayCommand(
                p => { CurrentView = new Dashboard(); PageTitle = "DASHBOARD"; },
                p => CanSeeReport);

            ShowMyAccountCommand = new RelayCommand(
                p => { CurrentView = new MyAccount(); PageTitle = "TÀI KHOẢN CỦA TÔI"; },
                p => CanSeeMyAccount);

            ShowDatTruocCommand = new RelayCommand(
                p => { CurrentView = new DatTruoc(); PageTitle = "ĐẶT CHỖ TRƯỚC"; },
                p => CanSeeDatTruoc);

            ShowQuanLyDatTruocCommand = new RelayCommand(
                p => { CurrentView = new QuanLyDatTruoc(); PageTitle = "QUẢN LÝ ĐẶT TRƯỚC"; },
                p => CanSeeQuanLyDatTruoc);

            LogoutCommand = new RelayCommand(
                p =>
                {
                    var result = MessageBox.Show(
                        "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?",
                        "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;

                    SessionManager.Logout();
                    var login = new DOANdotNET.Views.Login();
                    login.Show();
                    (p as Window)?.Close();
                },
                p => true);
        }
    }
}