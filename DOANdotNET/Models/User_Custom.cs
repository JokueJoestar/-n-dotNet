using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANdotNET.Models
{
    public partial class User
    {
        public string VaiTroHienThi
        {
            get
            {
                if (VaiTro == "admin") return "Quản trị viên";
                if (VaiTro == "nhanvien") return "Nhân viên";
                if (VaiTro == "khachhang") return "Khách hàng";
                return VaiTro;
            }
        }
        public string ChuCaiDau => string.IsNullOrEmpty(HoTen)
       ? "?" : HoTen.Trim()[0].ToString().ToUpper();
    }
}
