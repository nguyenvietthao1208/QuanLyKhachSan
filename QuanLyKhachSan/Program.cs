// =============================================
// Program.cs - Entry point
// =============================================

using System;
using System.Windows.Forms;
using QuanLyKhachSan.DAL;
using QuanLyKhachSan.GUI;

namespace QuanLyKhachSan
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Kiểm tra kết nối database
            if (!DBHelper.TestConnection())
            {
                MessageBox.Show(
                    "Không thể kết nối đến SQL Server!\n\n" +
                    "Vui lòng kiểm tra:\n" +
                    "1. SQL Server 2019 đang chạy\n" +
                    "2. Database 'QuanLyKhachSan' đã được tạo (chạy file SQL/CreateDatabase.sql)\n" +
                    "3. Chuỗi kết nối trong App.config\n\n" +
                    "Chuỗi kết nối hiện tại:\nData Source=.\\SQLEXPRESS;Initial Catalog=QuanLyKhachSan",
                    "Lỗi Kết Nối Database",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Hiển thị form đăng nhập
            var loginForm = new frmLogin();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                var mainForm = new frmMain(loginForm.LoggedInAccount);
                Application.Run(mainForm);
            }
        }
    }
}
