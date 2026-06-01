using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANdotNET.Models
{
    public partial class Transaction
    {
        
        public string TrangThai => ThoiGianRa.HasValue ? "Đã xong" : "Đang gửi";

        
        public double SoGioGui
        {
            get
            {
                if (ThoiGianRa.HasValue)
                {
                   
                    TimeSpan thoiGianDo = ThoiGianRa.Value - ThoiGianVao;
                    return thoiGianDo.TotalHours;
                }
                else
                {
                    TimeSpan thoiGianDo = DateTime.Now - ThoiGianVao;
                    return thoiGianDo.TotalHours;
                }
            }
        }
    }
}
