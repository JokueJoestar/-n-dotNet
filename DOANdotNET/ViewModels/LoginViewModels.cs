using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DOANdotNET.ViewModels.Services;
using DOANdotNET.Views;
using DOANdotNET.ViewModels.Helpers;
namespace DOANdotNET.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _userName;
        private string _password;
        private string _errorMsg;
        private readonly ParkingService _svc;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); } 
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { _errorMsg = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            _svc = new ParkingService();

            LoginCommand = new RelayCommand(
     p => Login(p as Window),
     p => true
 );
        }

        private void Login(Window win)
        {
            ErrorMsg = "";

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMsg = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            var user = _svc.Authenticate(UserName.Trim(), Password.Trim());

            if (user == null)
            {
                ErrorMsg = "Tài khoản hoặc mật khẩu sai!";
                return;
            }

          
            SessionManager.CurrentUser = user;

           
            MainWindow mainForm = new MainWindow();
            mainForm.Show();

          
            if (win != null)
            {
                win.Close();
            }
        }
    }
}
