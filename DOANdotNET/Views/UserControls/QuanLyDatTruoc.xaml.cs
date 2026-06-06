// Views/UserControls/QuanLyDatTruoc.xaml.cs
using System.Windows.Controls;
using DOANdotNET.ViewModels.UCViewModels;

namespace DOANdotNET.Views.UserControls
{
    // ✅ Phải là UserControl — khớp với XAML
    public partial class QuanLyDatTruoc : UserControl
    {
        public QuanLyDatTruoc()
        {
            InitializeComponent();

            // FIX: Chỉ load data lúc runtime, không phải lúc Designer render
            // Tránh XamlParseException khi Visual Studio mở XAML
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (s, e) =>
                {
                    if (DataContext is QuanLyDatTruocViewModel vm)
                        vm.KhoiTao();
                };
            }
        }
    }
}