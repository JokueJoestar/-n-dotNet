using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.ViewModels.Helpers;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using DOANdotNET.ViewModels.Services;
namespace DOANdotNET.ViewModels.UCViewModels
{
    public class MyAccountViewModel : BaseViewModel
    {
        // ===== THÔNG TIN HIỂN THỊ =====
        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set => SetProperty(ref _tenDangNhap, value);
        }

        private string _vaiTroHienThi;
        public string VaiTroHienThi
        {
            get => _vaiTroHienThi;
            set => SetProperty(ref _vaiTroHienThi, value);
        }

        // ===== LỆNH =====
        public ICommand ChangeMyPasswordCommand { get; set; }

        public MyAccountViewModel()
        {
            // 1. LẤY DỮ LIỆU THẬT TỪ NGƯỜI DÙNG ĐANG ĐĂNG NHẬP
            if (SessionManager.IsLoggedIn)
            {
                TenDangNhap = SessionManager.CurrentUser.HoTen; 
                VaiTroHienThi = SessionManager.CurrentUser.VaiTroHienThi; 
            }

            // 2. LOGIC LỆNH ĐỔI MẬT KHẨU (NỐI VỚI SQL SERVER)
            ChangeMyPasswordCommand = new RelayCommand(
                p =>
                {
                    // Giải nén 2 cái hộp PasswordBox từ giao diện gửi xuống
                    if (p is object[] passBoxes && passBoxes.Length == 2)
                    {
                        var oldPassBox = passBoxes[0] as PasswordBox;
                        var newPassBox = passBoxes[1] as PasswordBox;

                        if (oldPassBox == null || newPassBox == null) return;

                        string oldPass = oldPassBox.Password;
                        string newPass = newPassBox.Password;

                       
                        if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass))
                        {
                            MessageBox.Show("Vui lòng điền đầy đủ cả hai ô mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Gọi Service để đối chiếu và lưu xuống CSDL
                        var service = new ParkingService();
                        string userId = SessionManager.CurrentUser.ID;

                        bool doiThanhCong = service.DoiMatKhau(userId, oldPass, newPass);

                        if (!doiThanhCong)
                        {
                            
                            MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                      
                        MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Reset 2 ô nhập liệu
                        oldPassBox.Password = "";
                        newPassBox.Password = "";
                    }
                },
                p => SessionManager.IsLoggedIn 
            );
        }
    }
}