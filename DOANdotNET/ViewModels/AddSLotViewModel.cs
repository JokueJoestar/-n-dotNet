using DOANdotNET.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
namespace DOANdotNET.ViewModels
{
    public class AddSlotViewModel : BaseViewModel
    {
        
        private string _slotID;
        public string SlotID
        {
            get => _slotID;
            set => SetProperty(ref _slotID, value);
        }

        private string _khuVuc;
        public string KhuVuc
        {
            get => _khuVuc;
            set => SetProperty(ref _khuVuc, value);
        }

        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AddSlotViewModel()
        {
            ConfirmCommand = new RelayCommand(
                p =>
                {
                    if (p is Window currentWindow)
                    {
                        if (string.IsNullOrWhiteSpace(SlotID))
                        {
                            MessageBox.Show("Vui lòng nhập Mã vị trí!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        currentWindow.DialogResult = true;
                    }
                },
                p => true
            );

            CancelCommand = new RelayCommand(
                p =>
                {
                    if (p is Window currentWindow)
                    {
                        currentWindow.DialogResult = false; 
                    }
                },
                p => true
            );
        }
    }
}
