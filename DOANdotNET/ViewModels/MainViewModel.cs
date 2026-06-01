using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.Views.UserControls;
using System.Windows.Input;
using DOANdotNET.ViewModels;
using DOANdotNET.ViewModels.Services;
using System.Windows;
using DOANdotNET.ViewModels.Helpers;

namespace DOANdotNET.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // --- BIẾN ĐIỀU KHIỂN GIAO DIỆN CHÍNH ---
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // BIẾN HIỂN THỊ TIÊU ĐỀ TRANG TƯƠNG ỨNG
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }

        // --- KHAI BÁO CÁC COMMAND ĐIỀU HƯỚNG MENU ---
        public ICommand ShowParkingMapCommand { get; private set; }
        public ICommand ShowSearchCommand { get; private set; }
        public ICommand ShowStaffCommand { get; private set; }
        public ICommand ShowReportCommand { get; private set; }
       
        public ICommand ShowMyAccountCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand ShowVehicleTypeCommand { get; set; }

       
        public ICommand ThucHienLogicCommand { get; set; }

        // --- THÔNG TIN PHÂN QUYỀN VÀ HIỂN THỊ NGƯỜI DÙNG ---
        public string TenDangNhap => SessionManager.CurrentUser != null ? SessionManager.CurrentUser.HoTen : "Chưa đăng nhập";
        public string VaiTroHienThi => SessionManager.CurrentUser != null ? SessionManager.CurrentUser.VaiTroHienThi : "";

        public bool CanSeeReport => SessionManager.IsAdmin || SessionManager.IsNhanVien;
        public bool CanSeeStaff => SessionManager.IsAdmin;
        public bool CanSeeVehicleType => SessionManager.IsAdmin;
        public bool CanSeeMyAccount => SessionManager.CurrentUser != null;

        // HÀM KHỞI TẠO DUY NHẤT
        public MainViewModel()
        {
            

            // BẮT ĐẦU SỬA TỪ ĐÂY
            if (SessionManager.IsKhachHang)
            {
                CurrentView = new Search();
                PageTitle = "TÌM KIẾM XE";
            }
            else
            {
                CurrentView = new ParkingMap(); 
                PageTitle = "SƠ ĐỒ BÃI ĐỖ XE";
            }

            // --- KHỞI TẠO CHI TIẾT TỪNG LỆNH ĐIỀU HƯỚNG ---
            ShowParkingMapCommand = new RelayCommand(
                p => { CurrentView = new ParkingMap(); PageTitle = "SƠ ĐỒ BÃI ĐỖ XE"; },
                p => CanSeeReport
            );

            ShowSearchCommand = new RelayCommand(
                p => { CurrentView = new Search(); PageTitle = "TÌM KIẾM XE"; },
                p => true
            );

            ShowStaffCommand = new RelayCommand(
                p => { CurrentView = new StaffManager(); PageTitle = "QUẢN LÝ NHÂN SỰ"; },
                p => CanSeeStaff
            );

            ShowReportCommand = new RelayCommand(
                p => { CurrentView = new Report(); PageTitle = "BÁO CÁO THỐNG KÊ"; },
                p => CanSeeReport
            );

       

            ShowVehicleTypeCommand = new RelayCommand(
                p => { CurrentView = new VehicleTypeManager(); PageTitle = "QUẢN LÝ LOẠI XE"; },
                
                p => CanSeeVehicleType
            );

            ShowMyAccountCommand = new RelayCommand(
                p => { CurrentView = new MyAccount(); PageTitle = "TÀI KHOẢN CỦA TÔI"; },
                p => CanSeeMyAccount
            );

            // LOGIC NÚT ĐĂNG XUẤT
            LogoutCommand = new RelayCommand(
                 p =>
                 {
                    
                     var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?",
                                                  "Xác nhận",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);

                     
                     if (result == MessageBoxResult.Yes)
                     {
                         SessionManager.Logout();
                         Window currentWindow = p as Window;
                         if (currentWindow != null)
                         {
                             DOANdotNET.Views.Login loginForm = new DOANdotNET.Views.Login();
                             loginForm.Show();
                             currentWindow.Close();
                         }
                     }
                 },
                 p => true
             );

            ThucHienLogicCommand = new RelayCommand(
                p =>
                {
                    MessageBox.Show("Logic đã được gọi thành công từ ViewModel!", "Chuẩn MVVM");
                },
                p => true
            );
        }
    }
}