using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DOANdotNET.ViewModels;
using Microsoft.Win32;

namespace DOANdotNET.Views
{
    /// <summary>
    /// Interaction logic for CheckIn.xaml
    /// </summary>
    public partial class CheckIn : Window
    {

        // ✅ Nhận idSlot từ bên ngoài
        
        
            public CheckIn(string idDoXe)
            {
                InitializeComponent();

                // Ném idDoXe vào ViewModel
                this.DataContext = new DOANdotNET.ViewModels.CheckInViewModel(idDoXe);
            }
       


    }
}
