using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANdotNET.Models
{
    public partial class LoaiXe
    {
        // ── Thêm GiaDoXe alias PhiMoiGio ──────────────────────────
        public decimal GiaDoXe
        {
            get => PhiMoiGio;
            set => PhiMoiGio = value;
        }

        // ── SoXeDangGui dùng field thông thường ───────────────────
        private int _soXeDangGui;
        public int SoXeDangGui
        {
            get => _soXeDangGui;
            set
            {
                _soXeDangGui = value;
            }
        }
    }
}