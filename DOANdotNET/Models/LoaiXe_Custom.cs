using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOANdotNET.ViewModels.Helpers;

namespace DOANdotNET.Models
{
    public partial class LoaiXe : BaseViewModel
    {
        private int _soXeDangGui;
        public int SoXeDangGui
        {
            get => _soXeDangGui;
            set => SetProperty(ref _soXeDangGui, value);
        }
    }
}
