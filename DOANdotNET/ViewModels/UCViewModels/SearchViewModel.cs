using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.ViewModels.Helpers;
using System.Windows.Input;
using DOANdotNET.ViewModels.Services;
namespace DOANdotNET.ViewModels.UCViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly ParkingService _service;

        // ===== 1. BIẾN ĐẦU VÀO TÌM KIẾM =====
        private string _searchBienSo;
        public string SearchBienSo
        {
            get => _searchBienSo;
            set => SetProperty(ref _searchBienSo, value?.ToUpper()); 
        }

        // ===== 2. CÁC BIẾN ĐIỀU KHIỂN TRẠNG THÁI UI =====
        private bool _isSearched;
        public bool IsSearched
        {
            get => _isSearched;
            set
            {
                SetProperty(ref _isSearched, value);
                OnPropertyChanged(nameof(IsNotSearched)); 
            }
        }

        public bool IsNotSearched => !IsSearched;

        private bool _isFound;
        public bool IsFound
        {
            get => _isFound;
            set => SetProperty(ref _isFound, value);
        }

        private bool _isNotFound;
        public bool IsNotFound
        {
            get => _isNotFound;
            set => SetProperty(ref _isNotFound, value);
        }

        // ===== 3. CÁC BIẾN LƯU KẾT QUẢ ĐẦU RA =====
        private string _resultBienSo;
        public string ResultBienSo { get => _resultBienSo; set => SetProperty(ref _resultBienSo, value); }

        private string _resultIDDoXe;
        public string ResultIDDoXe { get => _resultIDDoXe; set => SetProperty(ref _resultIDDoXe, value); }

        private string _resultTenKhach;
        public string ResultTenKhach { get => _resultTenKhach; set => SetProperty(ref _resultTenKhach, value); }

        private string _resultKhuVuc;
        public string ResultKhuVuc { get => _resultKhuVuc; set => SetProperty(ref _resultKhuVuc, value); }

        private DateTime _resultGioVao;
        public DateTime ResultGioVao { get => _resultGioVao; set => SetProperty(ref _resultGioVao, value); }

        private string _resultSoDienThoai;
        public string ResultSoDienThoai { get => _resultSoDienThoai; set => SetProperty(ref _resultSoDienThoai, value); }

        // ===== 4. LỆNH (COMMANDS) =====
        public ICommand SearchCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

        public SearchViewModel()
        {
            _service = new ParkingService(); 

            // Trạng thái ban đầu
            IsSearched = false;
            IsFound = false;
            IsNotFound = false;

            // NÚT TÌM KIẾM
            SearchCommand = new RelayCommand(
                p =>
                {
                    if (string.IsNullOrWhiteSpace(SearchBienSo)) return;

                    IsSearched = true;

                    //  GỌI SQL: Tìm xe đang đỗ dựa vào biển số
                    var xe = _service.FindByBienSo(SearchBienSo);

                    if (xe != null)
                    {
                        
                        IsFound = true;
                        IsNotFound = false;

                        
                        ResultBienSo = xe.BienSoXe;
                        ResultIDDoXe = xe.IDDoXe;
                        ResultTenKhach = xe.TenKhachHang;
                        ResultKhuVuc = xe.KhuVuc;
                        ResultGioVao = xe.ThoiGianVao ?? DateTime.Now;
                        ResultSoDienThoai = xe.SoDienThoai;
                    }
                    else
                    {
                        
                        IsFound = false;
                        IsNotFound = true;
                    }
                },
                
                p => !string.IsNullOrWhiteSpace(SearchBienSo)
            );

            // NÚT LÀM TRỐNG
            ClearSearchCommand = new RelayCommand(p =>
            {
                SearchBienSo = "";
                IsSearched = false;
                IsFound = false;
                IsNotFound = false;
            });
        }
    }
}
