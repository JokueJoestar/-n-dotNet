using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.ViewModels.Helpers;

namespace DOANdotNET.Models
{

    public partial class ParkingSlot : BaseViewModel
    {
        
        public decimal TinhPhiDuTinh(decimal phiMoiGio)
        {
            if (ThoiGianVao == null) return 0;

           
            TimeSpan thoiGianDo = DateTime.Now - ThoiGianVao.Value;
            double soGio = thoiGianDo.TotalHours;

            return (decimal)Math.Ceiling(soGio) * phiMoiGio;
        }

 
        public double SoGioGui
        {
            get
            {
                if (ThoiGianVao == null) return 0;
                TimeSpan thoiGianDo = DateTime.Now - ThoiGianVao.Value;
                return thoiGianDo.TotalHours;
            }
        }
    }
}
