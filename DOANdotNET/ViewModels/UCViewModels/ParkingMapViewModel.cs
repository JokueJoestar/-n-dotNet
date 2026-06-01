using DOANdotNET.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DOANdotNET.Models;
using DOANdotNET.Views;
using DOANdotNET.ViewModels.Services;
using System.Windows;


namespace DOANdotNET.ViewModels.UCViewModels
{
    public class ParkingMapViewModel : BaseViewModel
    {
        private readonly ParkingService _service;

        // ===== PROPERTIES =====
        private ObservableCollection<ParkingSlot> _listSlots;
        public ObservableCollection<ParkingSlot> ListSlots
        {
            get => _listSlots;
            set => SetProperty(ref _listSlots, value);
        }

        private ParkingSlot _selectedSlot;
        public ParkingSlot SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                SetProperty(ref _selectedSlot, value);
               
                OnPropertyChanged(nameof(ThanhTienDuTinh));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private int _slotTrong;
        public int SlotTrong
        {
            get => _slotTrong;
            set => SetProperty(ref _slotTrong, value);
        }

        private int _slotCoXe;
        public int SlotCoXe
        {
            get => _slotCoXe;
            set => SetProperty(ref _slotCoXe, value);
        }

        // Phí dự tính — tính realtime
        public decimal ThanhTienDuTinh
            => SelectedSlot != null ? _service.TinhPhiDuTinh(SelectedSlot) : 0;

        // CanAddSlot — chỉ Admin mới thấy nút Thêm/Xóa
        public bool CanAddSlot
            => ViewModels.Services.SessionManager.IsAdmin;

        // ===== COMMANDS =====
        public ICommand CheckInCommand { get; }
        public ICommand CheckOutCommand { get; }
        public ICommand AddSlotCommand { get; }
        public ICommand RemoveSlotCommand { get; }
        public ICommand RefreshCommand { get; } 

        // ===== CONSTRUCTOR =====
        public ParkingMapViewModel()
        {
            _service = new ParkingService();
            ListSlots = new ObservableCollection<ParkingSlot>();

           
            CheckInCommand = new RelayCommand(_ => DoCheckIn(),
                _ => SelectedSlot?.TrangThai == "Trống");

            
            CheckOutCommand = new RelayCommand(_ => DoCheckOut(),
                _ => SelectedSlot?.TrangThai == "Đang gửi");

            AddSlotCommand = new RelayCommand(_ => DoAddSlot(),
                _ => CanAddSlot);

            RemoveSlotCommand = new RelayCommand(_ => DoRemoveSlot(),
                _ => SelectedSlot != null && CanAddSlot);

            RefreshCommand = new RelayCommand(_ => LoadData()); 

            LoadData();
        }

        // ===== LOAD DỮ LIỆU =====
        private void LoadData()
        {
            ListSlots.Clear();
            foreach (var slot in _service.GetParkingSlots())
                ListSlots.Add(slot);

            SlotTrong = _service.CountTrong();
            SlotCoXe = _service.CountCoXe();

            // Reset selected
            SelectedSlot = null;
            OnPropertyChanged(nameof(ThanhTienDuTinh));
        }

        // ===== CHECK IN =====
        private void DoCheckIn()
        {
            if (SelectedSlot == null) return;

            var dlg = new CheckIn(SelectedSlot.IDDoXe);
            if (dlg.ShowDialog() == true)
            {
                LoadData(); 
                StatusMessage = $"✅ Xe đã vào vị trí {SelectedSlot?.IDDoXe}";
            }
        }

        // ===== CHECK OUT =====
        private void DoCheckOut()
        {
            if (SelectedSlot == null) return;

            var dlg = new CheckOut(SelectedSlot.IDDoXe);
            if (dlg.ShowDialog() == true)
            {
                LoadData(); 
                StatusMessage = "✅ Xe đã ra bãi thành công";
            }
        }

        private void DoAddSlot()
        {
            var dlg = new AddSlot();
            var addViewModel = new AddSlotViewModel();
            dlg.DataContext = addViewModel;

            if (dlg.ShowDialog() == true)
            {
                string newSlotID = addViewModel.SlotID;
                string newKhuVuc = addViewModel.KhuVuc;

                // GỌI THẲNG XUỐNG SERVICE ĐỂ LƯU VÀO HỆ THỐNG
                bool ok = _service.AddSlot(newSlotID, newKhuVuc);

                if (ok)
                {
                    LoadData(); // Hàm này sẽ tự động quét lại toàn bộ vị trí từ Service vẽ lên UI
                    StatusMessage = $"✅ Đã thêm vị trí mới: {newSlotID}";
                }
                else
                {
                    MessageBox.Show("Mã vị trí đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // ===== XÓA VỊ TRÍ =====
        private void DoRemoveSlot()
        {
            if (SelectedSlot == null) return;

          
            if (SelectedSlot.TrangThai == "Đang gửi")
            {
                MessageBox.Show("Không thể xóa vị trí đang có xe!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận xóa vị trí {SelectedSlot.IDDoXe}?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            // GỌI THẲNG XUỐNG SERVICE ĐỂ XÓA TẬN GỐC
            bool ok = _service.RemoveSlot(SelectedSlot.IDDoXe);

            if (ok)
            {
                LoadData();
                StatusMessage = "✅ Đã xóa vị trí";
            }
            else
            {
                MessageBox.Show("Không thể xóa vị trí này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
 }