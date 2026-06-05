using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;
using System.Windows.Input;

namespace DOANdotNET.ViewModels.UCViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ParkingService _service;
        private readonly DatTruocService _datTruocService;

        private int _tongSlot;
        public int TongSlot
        {
            get => _tongSlot;
            set => SetProperty(ref _tongSlot, value);
        }

        private int _dangGui;
        public int DangGui
        {
            get => _dangGui;
            set => SetProperty(ref _dangGui, value);
        }

        private int _oTrong;
        public int OTrong
        {
            get => _oTrong;
            set => SetProperty(ref _oTrong, value);
        }

        private decimal _doanhThuHomNay;
        public decimal DoanhThuHomNay
        {
            get => _doanhThuHomNay;
            set => SetProperty(ref _doanhThuHomNay, value);
        }

        private int _luotRaHomNay;
        public int LuotRaHomNay
        {
            get => _luotRaHomNay;
            set => SetProperty(ref _luotRaHomNay, value);
        }

        private int _soDangDatTruoc;
        public int SoDangDatTruoc
        {
            get => _soDangDatTruoc;
            set => SetProperty(ref _soDangDatTruoc, value);
        }

        public ICommand RefreshCommand { get; }

        public DashboardViewModel()
        {
            _service = new ParkingService();
            _datTruocService = new DatTruocService();

            RefreshCommand = new RelayCommand(_ => LoadData());
            LoadData();
        }

        private void LoadData()
        {
            TongSlot = _service.CountTongSlot();
            DangGui = _service.CountCoXe();
            OTrong = _service.CountTrong();
            DoanhThuHomNay = _service.GetDoanhThuHomNay();
            LuotRaHomNay = _service.GetLuotRaHomNay();
            SoDangDatTruoc = _datTruocService.DemDangDatTruoc();
        }
    }
}