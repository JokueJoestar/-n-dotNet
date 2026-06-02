using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace DOANdotNET
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!EnsureDatabase())
            {
                Current.Shutdown();
            }
        }

        private bool EnsureDatabase()
        {
            try
            {
                // Kiểm tra database đã tồn tại chưa
                string masterConn = @"data source=(localdb)\MSSQLLocalDB;initial catalog=master;integrated security=True;";
                using (var conn = new SqlConnection(masterConn))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM sys.databases WHERE name = 'QL_BaiDoXe'", conn);
                    int exists = (int)cmd.ExecuteScalar();

                    if (exists == 0)
                    {
                        // Chạy file SQL để tạo database
                        string sqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QL_BaiDoXe.sql");
                        if (!File.Exists(sqlPath))
                        {
                            MessageBox.Show(
                                "Không tìm thấy file QL_BaiDoXe.sql!\n" +
                                "Vui lòng đặt file này cùng thư mục với ứng dụng.",
                                "Thiếu file", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }

                        string sql = File.ReadAllText(sqlPath);
                        // Tách GO statements
                        foreach (var batch in sql.Split(new[] { "\r\nGO", "\nGO", " GO" },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (string.IsNullOrWhiteSpace(batch)) continue;
                            using (var batchCmd = new SqlCommand(batch, conn))
                                batchCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("✅ Database đã được tạo thành công!",
                            "Khởi tạo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "❌ Không thể khởi tạo Database!\n\n" + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}