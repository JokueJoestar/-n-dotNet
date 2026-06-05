using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DOANdotNET.Models;
using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;

namespace DOANdotNET.ViewModels.UCViewModels
{
    public class DatTruocViewModel : BaseViewModel
    {
        private readonly DatTruocService _svc = new DatTruocService();
        private readonly ParkingService _parkingSvc = new ParkingService();

        // ── Tab đang hiển thị (0 = Đặt mới, 1 = Lịch sử) ───────────
        private int _tabIndex;
        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                SetProperty(ref _tabIndex, value);
                OnPropertyChanged(nameof(IsTabDat));
                OnPropertyChanged(nameof(IsTabLichSu));
            }
        }
        public bool IsTabDat => _tabIndex == 0;
        public bool IsTabLichSu => _tabIndex == 1;

        // ── Form đặt trước ───────────────────────────────────────────
        private ObservableCollection<string> _danhSachKhuVuc;
        public ObservableCollection<string> DanhSachKhuVuc
        {
            get => _danhSachKhuVuc;
            set => SetProperty(ref _danhSachKhuVuc, value);
        }

        private string _khuVucChon;
        public string KhuVucChon
        {
            get => _khuVucChon;
            set
            {
                SetProperty(ref _khuVucChon, value);
                OnPropertyChanged(nameof(SoSlotThongBao));
            }
        }

        private ObservableCollection<LoaiXe> _danhSachLoaiXe;
        public ObservableCollection<LoaiXe> DanhSachLoaiXe
        {
            get => _danhSachLoaiXe;
            set => SetProperty(ref _danhSachLoaiXe, value);
        }

        private LoaiXe _loaiXeChon;
        public LoaiXe LoaiXeChon
        {
            get => _loaiXeChon;
            set => SetProperty(ref _loaiXeChon, value);
        }

        private DateTime _ngayDen = DateTime.Today;
        public DateTime NgayDen
        {
            get => _ngayDen;
            set => SetProperty(ref _ngayDen, value);
        }

        private string _gioDen = DateTime.Now.AddHours(1).ToString("HH:mm");
        public string GioDen
        {
            get => _gioDen;
            set => SetProperty(ref _gioDen, value);
        }

        public string SoSlotThongBao
        {
            get
            {
                if (string.IsNullOrEmpty(KhuVucChon)) return "";
                int so = _svc.DemSlotTrong(KhuVucChon);
                return so > 0 ? $"Còn {so} chỗ trống" : "Hết chỗ trống";
            }
        }

        private string _thongBaoDat;
        public string ThongBaoDat
        {
            get => _thongBaoDat;
            set => SetProperty(ref _thongBaoDat, value);
        }

        private bool _datThanhCong;
        public bool DatThanhCong
        {
            get => _datThanhCong;
            set => SetProperty(ref _datThanhCong, value);
        }

        // ── Lịch sử ─────────────────────────────────────────────────
        private ObservableCollection<DatTruoc> _lichSu;
        public ObservableCollection<DatTruoc> LichSu
        {
            get => _lichSu;
            set => SetProperty(ref _lichSu, value);
        }

        private DatTruoc _bookingChon;
        public DatTruoc BookingChon
        {
            get => _bookingChon;
            set
            {
                SetProperty(ref _bookingChon, value);
                OnPropertyChanged(nameof(CoTheHuy));
            }
        }

        public bool CoTheHuy => _bookingChon?.CoTheHuy == true;

        // ── Commands ─────────────────────────────────────────────────
        public ICommand DatTruocCommand { get; }
        public ICommand HuyCommand { get; }
        public ICommand LamMoiCommand { get; }
        public ICommand XemLichSuCommand { get; }
        public ICommand QuayLaiCommand { get; }

        public DatTruocViewModel()
        {
            TaiDuLieuForm();
            TaiLichSu();

            DatTruocCommand = new RelayCommand(
                _ => ThucHienDat(),
                _ => KhuVucChon != null && LoaiXeChon != null
                                        && !string.IsNullOrWhiteSpace(GioDen)
            );

            HuyCommand = new RelayCommand(
    param =>
    {
        // Nếu bấm nút Hủy trong card → param là booking đó
        // Nếu bấm nút Hủy ở action bar bên dưới → param null, dùng _bookingChon
        var booking = param as DatTruoc ?? _bookingChon;
        if (booking == null) return;

        var res = MessageBox.Show(
            $"Hủy đặt trước {booking.MaDatTruoc}?\n" +
            $"Khu vực: {booking.KhuVuc}  —  {booking.ThoiGianDenHienThi}",
            "Xác nhận hủy",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (res != MessageBoxResult.Yes) return;

        bool ok = _svc.HuyDatTruoc(booking.MaDatTruoc, SessionManager.CurrentUser.ID);
        if (ok)
        {
            MessageBox.Show("Đã hủy đặt trước.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
            TaiLichSu();
            BookingChon = null;
        }
        else
        {
            MessageBox.Show("Không thể hủy. Vui lòng thử lại.", "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    },
    param =>
    {
        var booking = param as DatTruoc ?? _bookingChon;
        return booking?.CoTheHuy == true;
    }
);

            LamMoiCommand = new RelayCommand(_ => ResetForm());

            XemLichSuCommand = new RelayCommand(_ =>
            {
                TaiLichSu();
                TabIndex = 1;
            });

            QuayLaiCommand = new RelayCommand(_ => TabIndex = 0);
        }

        // ── Private helpers ──────────────────────────────────────────
        private void TaiDuLieuForm()
        {
            DanhSachKhuVuc = new ObservableCollection<string>(_svc.LayKhuVucConTrong());
            DanhSachLoaiXe = new ObservableCollection<LoaiXe>(_parkingSvc.GetAllLoaiXe());
        }

        private void TaiLichSu()
        {
            var id = SessionManager.CurrentUser?.ID;
            if (id == null) return;
            LichSu = new ObservableCollection<DatTruoc>(_svc.LayLichSu(id));
        }

        private void ResetForm()
        {
            KhuVucChon = null;
            LoaiXeChon = null;
            NgayDen = DateTime.Today;
            GioDen = DateTime.Now.AddHours(1).ToString("HH:mm");
            ThongBaoDat = "";
            DatThanhCong = false;
            TaiDuLieuForm();
        }

        private void ThucHienDat()
        {
            if (!TimeSpan.TryParse(GioDen, out var ts))
            {
                ThongBaoDat = "⚠ Giờ không hợp lệ. Nhập theo định dạng HH:mm";
                DatThanhCong = false;
                return;
            }

            var thoiGianDen = NgayDen.Date + ts;
            if (thoiGianDen <= DateTime.Now)
            {
                ThongBaoDat = "⚠ Thời gian đến phải sau thời điểm hiện tại";
                DatThanhCong = false;
                return;
            }

            var ma = _svc.TaoDatTruoc(
                SessionManager.CurrentUser.ID,
                KhuVucChon,
                LoaiXeChon.MaLoaiXe,
                thoiGianDen);

            if (ma == null)
            {
                ThongBaoDat = $"⚠ {KhuVucChon} hiện không còn chỗ trống";
                DatThanhCong = false;
                return;
            }

            // Popup thông báo thành công có đủ thông tin
            string giaHienThi = LoaiXeChon.GiaDoXe > 0
                ? LoaiXeChon.GiaDoXe.ToString("N0") + " đ/giờ"
                : "theo bảng giá";

            MessageBox.Show(
                $"✅  Đặt chỗ thành công!\n\n" +
                $"Mã đặt trước  :  {ma}\n" +
                $"Khu vực       :  {KhuVucChon}\n" +
                $"Loại xe       :  {LoaiXeChon.TenLoaiXe}\n" +
                $"Thời gian đến :  {thoiGianDen:HH:mm  dd/MM/yyyy}\n" +
                $"Giá đỗ xe     :  {giaHienThi}\n\n" +
                $"⚠  Chỗ được giữ 30 phút. Vui lòng đến đúng giờ.",
                "Đặt chỗ thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ThongBaoDat = $"✓  Mã {ma} — đến lúc {thoiGianDen:HH:mm dd/MM}";
            DatThanhCong = true;

            // Tự động làm mới form sau khi đặt thành công
            ResetForm();
        }
    }
}