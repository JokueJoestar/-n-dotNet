using DOANdotNET.Models;
using System;
using System.Linq;
using System.Windows;

namespace DOANdotNET.Views
{
    public partial class ReportWindow : Window
    {
        private QL_BaiDoXeEntities _db = new QL_BaiDoXeEntities();

        public ReportWindow()
        {
            InitializeComponent();

            dpFrom.SelectedDate = DateTime.Now.AddDays(-7);
            dpTo.SelectedDate = DateTime.Now;

            LoadReport();
        }

        private void BtnLoadReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                DateTime fromDate = dpFrom.SelectedDate ?? DateTime.Now.AddDays(-7);
                DateTime toDate = dpTo.SelectedDate ?? DateTime.Now;

                toDate = toDate.AddDays(1);

                var data = _db.Transactions
                    .Where(x =>
                        x.ThoiGianRa != null &&
                        x.ThoiGianRa >= fromDate &&
                        x.ThoiGianRa < toDate)
                    .OrderByDescending(x => x.ThoiGianRa)
                    .ToList();

                dgReport.ItemsSource = data;

                decimal totalRevenue = data.Sum(x => x.ThanhTien);

                txtTotalRevenue.Text = totalRevenue.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải báo cáo:\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}