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

        private string _filterTrangThai = "Tất cả";
        public string FilterTrangThai
        {
            get => _filterTrangThai;
            set { SetProperty(ref _filterTrangThai, value); TaiDuLieu(); }
        }

        public string[] DanhSachFilter { get; } =
            { "Tất cả", "Chờ xử lý", "Đã xác nhận", "Đã hủy", "Hoàn thành" };

        private ObservableCollection<DatTruoc> _danhSach;
        public ObservableCollection<DatTruoc> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

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
                // FIX: Buộc WPF re-evaluate CanExecute của tất cả command
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool CoChonBooking => _chon != null;
        public bool CanXacNhan => _chon?.TrangThai == "Chờ xử lý";
        public bool CanHuy => _chon?.CoTheHuy == true;

        public ICommand LamMoiCommand { get; }
        public ICommand XacNhanCommand { get; }
        public ICommand HuyCommand { get; }

        public QuanLyDatTruocViewModel()
        {
            DanhSach = new ObservableCollection<DatTruoc>();

            LamMoiCommand = new RelayCommand(_ => TaiDuLieu());

            XacNhanCommand = new RelayCommand(
                _ =>
                {
                    if (!_svc.XacNhanDatTruoc(_chon.MaDatTruoc))
                    {
                        MessageBox.Show("Không thể xác nhận. Booking đã thay đổi trạng thái.",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        TaiDuLieu(); return;
                    }
                    MessageBox.Show(
                        $"✅ Đã xác nhận đặt trước {_chon.MaDatTruoc}\n" +
                        $"Khách: {_chon.User?.HoTen}\nSlot: {_chon.IDDoXe} — {_chon.KhuVuc}",
                        "Xác nhận thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    TaiDuLieu();
                    ChonBooking = null;
                },
                _ => CanXacNhan   // FIX: lambda này được re-check nhờ InvalidateRequerySuggested
            );

            HuyCommand = new RelayCommand(
                _ =>
                {
                    var r = MessageBox.Show(
                        $"Hủy đặt trước {_chon.MaDatTruoc}?\n" +
                        $"Khách: {_chon.User?.HoTen}\n" +
                        $"Khu vực: {_chon.KhuVuc} — {_chon.ThoiGianDenHienThi}",
                        "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (r != MessageBoxResult.Yes) return;

                    if (_svc.HuyBoiAdmin(_chon.MaDatTruoc))
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

        public void KhoiTao() => TaiDuLieu();

        private void TaiDuLieu()
        {
            string filter = FilterTrangThai == "Tất cả" ? null : FilterTrangThai;
            DanhSach = new ObservableCollection<DatTruoc>(_svc.LayTatCa(filter));
        }
    }
}