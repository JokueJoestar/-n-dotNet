using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.Models;

namespace DOANdotNET.ViewModels.Services
{
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }

        // ===== PHÂN QUYỀN =====
        public static bool IsAdmin => CurrentUser?.VaiTro == "admin";
        public static bool IsNhanVien => CurrentUser?.VaiTro == "nhanvien";
        public static bool IsKhachHang => CurrentUser?.VaiTro == "khachhang";

        //  kiểm tra đã đăng nhập chưa
        public static bool IsLoggedIn => CurrentUser != null;

        // Admin hoặc NhanVien đều có thể thao tác bãi xe
        public static bool CanOperate => IsAdmin || IsNhanVien;

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
