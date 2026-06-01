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

namespace DOANdotNET.Views
{
    /// <summary>
    /// Interaction logic for AddSlot.xaml
    /// </summary>
    public partial class AddSlot : Window
    {
        public AddSlot()
        {
            InitializeComponent();
            this.DataContext = new DOANdotNET.ViewModels.AddSlotViewModel();
        }

    } 
}
    
