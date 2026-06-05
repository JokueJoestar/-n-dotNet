using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DOANdotNET.Models;
using DOANdotNET.ViewModels.Helpers;
using DOANdotNET.ViewModels.Services;

namespace DOANdotNET.ViewModels.UCViewModels
{
    public class QuanLyDatTruocViewModel : BaseViewModel
    {
        private readonly DatTruocService _svc = new DatTruocService();

        // ── Danh sách & bộ lọc ──────────────────────────────────────
        private ObservableCollection<DatTruoc> _danhSach;
        public ObservableCollection<DatTruoc> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private string _filterTrangThai = "Tất cả";
        public string FilterTrangThai
        {
            get => _filterTrangThai;
            set
            {
                SetProperty(ref _filterTrangThai, value);
                TaiDuLieu();
            }
        }

        public string[] DanhSachFilter { get; } =
            { "Tất cả", "Chờ xử lý", "Đã xác nhận", "Đã hủy", "Hoàn thành" };

        // ── Booking đang chọn ────────────────────────────────────────
        private DatTruoc _chon;
        public DatTruoc ChonBooking
        {
            get => _chon;
            set
            {
                SetProperty(ref _chon, value);
                OnPropertyChanged(nameof(CanXacNhan));
                OnPropertyChanged(nameof(CanHuy));
                OnPropertyChanged(nameof(CoChonBooking));
            }
        }

        public bool CoChonBooking => _chon != null;
        public bool CanXacNhan => _chon?.TrangThai == "Chờ xử lý";
        public bool CanHuy => _chon?.CoTheHuy == true;

        // ── Commands ─────────────────────────────────────────────────
        public ICommand LamMoiCommand { get; }
        public ICommand XacNhanCommand { get; }
        public ICommand HuyCommand { get; }

        public QuanLyDatTruocViewModel()
        {
            TaiDuLieu();

            LamMoiCommand = new RelayCommand(_ => TaiDuLieu());

            XacNhanCommand = new RelayCommand(
                _ =>
                {
                    if (!_svc.XacNhanDatTruoc(_chon.MaDatTruoc))
                    {
                        MessageBox.Show("Không thể xác nhận. Booking có thể đã thay đổi trạng thái.",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        TaiDuLieu();
                        return;
                    }
                    MessageBox.Show(
                        $"✅ Đã xác nhận đặt trước {_chon.MaDatTruoc}\n" +
                        $"Khách: {_chon.User?.HoTen}\n" +
                        $"Slot: {_chon.IDDoXe}  —  {_chon.KhuVuc}",
                        "Xác nhận thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    TaiDuLieu();
                    ChonBooking = null;
                },
                _ => CanXacNhan
            );

            HuyCommand = new RelayCommand(
                _ =>
                {
                    var r = MessageBox.Show(
                        $"Hủy đặt trước {_chon.MaDatTruoc}?\n" +
                        $"Khách: {_chon.User?.HoTen}\n" +
                        $"Khu vực: {_chon.KhuVuc}  —  {_chon.ThoiGianDenHienThi}",
                        "Xác nhận hủy",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (r != MessageBoxResult.Yes) return;

                    if (_svc.HuyDatTruoc(_chon.MaDatTruoc, _chon.IDKhachHang))
                    {
                        MessageBox.Show("Đã hủy đặt trước.", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        TaiDuLieu();
                        ChonBooking = null;
                    }
                    else
                    {
                        MessageBox.Show("Không thể hủy. Vui lòng thử lại.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                },
                _ => CanHuy
            );
        }

        private void TaiDuLieu()
        {
            string filter = FilterTrangThai == "Tất cả" ? null : FilterTrangThai;
            DanhSach = new ObservableCollection<DatTruoc>(_svc.LayTatCa(filter));
        }
    }
}