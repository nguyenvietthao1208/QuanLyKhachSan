// =============================================
// GUI - Form Đăng Nhập (frmLogin.cs)
// =============================================
// Tạo file này trong VS: Add > Windows Form > đặt tên frmLogin
// Copy toàn bộ code vào frmLogin.cs
// Designer: tạo các control hoặc dùng code-behind bên dưới

using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class frmLogin : Form
    {
        // ── Controls ──────────────────────────────────────────
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlForm;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private CheckBox chkShowPass;
        private Button btnLogin;
        private Label lblError;
        private PictureBox picLogo;

        // ── State ─────────────────────────────────────────────
        public AccountDTO LoggedInAccount { get; private set; }

        // Màu chủ đạo
        private readonly Color PrimaryColor   = Color.FromArgb(106, 90, 205);   // Tím
        private readonly Color SecondaryColor = Color.FromArgb(147, 112, 219);
        private readonly Color AccentColor    = Color.FromArgb(255, 255, 255);
        private readonly Color DangerColor    = Color.FromArgb(220, 53, 69);
        private readonly Color BgColor        = Color.FromArgb(245, 245, 250);

        public frmLogin()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text            = "Đăng Nhập - Quản Lý Khách Sạn";
            this.Size            = new Size(420, 560);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.BackColor       = BgColor;
            this.Font            = new Font("Segoe UI", 9.5f);

            // Header
            pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 160,
                BackColor = PrimaryColor
            };

            lblTitle = new Label
            {
                Text      = "QUẢN LÝ KHÁCH SẠN",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize  = false,
                Width     = 400,
                Height    = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(0, 70)
            };

            lblSubtitle = new Label
            {
                Text      = "Vui lòng đăng nhập để tiếp tục",
                ForeColor = Color.FromArgb(200, 200, 255),
                Font      = new Font("Segoe UI", 9),
                AutoSize  = false,
                Width     = 400,
                Height    = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(0, 108)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // Form panel
            pnlForm = new Panel
            {
                Location  = new Point(30, 180),
                Size      = new Size(340, 330),
                BackColor = BgColor
            };

            // Username
            lblUsername = new Label
            {
                Text     = "Tên đăng nhập",
                Location = new Point(0, 10),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtUsername = new TextBox
            {
                Location    = new Point(0, 32),
                Size        = new Size(340, 35),
                Font        = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text        = "admin"
            };
            StyleTextBox(txtUsername);

            // Password
            lblPassword = new Label
            {
                Text     = "Mật khẩu",
                Location = new Point(0, 85),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtPassword = new TextBox
            {
                Location      = new Point(0, 107),
                Size          = new Size(340, 35),
                Font          = new Font("Segoe UI", 11),
                BorderStyle   = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                Text          = "admin123"
            };
            StyleTextBox(txtPassword);

            chkShowPass = new CheckBox
            {
                Text     = "Hiển thị mật khẩu",
                Location = new Point(0, 150),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            chkShowPass.CheckedChanged += (s, e) =>
                txtPassword.UseSystemPasswordChar = !chkShowPass.Checked;

            // Error label
            lblError = new Label
            {
                Text      = "",
                ForeColor = DangerColor,
                Location  = new Point(0, 178),
                AutoSize  = false,
                Size      = new Size(340, 40),
                Font      = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Login button
            btnLogin = new Button
            {
                Text      = "ĐĂNG NHẬP",
                Location  = new Point(0, 220),
                Size      = new Size(340, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += btnLogin_Click;

            // Enter key
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnLogin_Click(null, null);
            };
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) txtPassword.Focus();
            };

            pnlForm.Controls.AddRange(new Control[]
            {
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                chkShowPass, lblError, btnLogin
            });

            this.Controls.AddRange(new Control[] { pnlHeader, pnlForm });
        }

        private void StyleTextBox(TextBox txt)
        {
            txt.BackColor = Color.White;
        }

        private void SetupUI()
        {
            // Hover effect on button
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = SecondaryColor;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = PrimaryColor;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            btnLogin.Enabled = false;
            btnLogin.Text = "Đang đăng nhập...";

            try
            {
                var bll = new AccountBLL();
                var acc = bll.Login(txtUsername.Text.Trim(), txtPassword.Text);

                if (acc == null)
                {
                    lblError.Text = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    txtPassword.Focus();
                    txtPassword.SelectAll();
                }
                else
                {
                    LoggedInAccount = acc;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }
    }
}
