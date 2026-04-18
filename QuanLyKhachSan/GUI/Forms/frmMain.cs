// =============================================
// GUI - Form Chính (frmMain.cs)
// MDI Container với Sidebar navigation
// =============================================

using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class frmMain : Form
    {
        // ── Controls ──────────────────────────────────────────
        private Panel pnlHeader;
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblAppName;
        private Label lblUserName;
        private Button btnUserMenu;
        private ContextMenuStrip menuUser;
        private ToolStripMenuItem mnuProfile;
        private ToolStripMenuItem mnuLogout;

        // Nav buttons
        private Button btnSoDo;
        private Button btnKhachHang;
        private Button btnHoaDon;
        private Button btnThietLap;
        private Button btnBaoCao;

        private Button _activeButton;

        // Colors
        private readonly Color HeaderColor  = Color.FromArgb(106, 90, 205);
        private readonly Color SidebarColor = Color.FromArgb(42, 42, 65);
        private readonly Color NavHover     = Color.FromArgb(60, 60, 90);
        private readonly Color NavActive    = Color.FromArgb(106, 90, 205);
        private readonly Color ContentBg    = Color.FromArgb(240, 240, 248);

        public AccountDTO CurrentAccount { get; private set; }

        // Child panels
        private ucSoDo    _ucSoDo;
        private ucKhachHang _ucKhachHang;
        private ucHoaDon  _ucHoaDon;
        private ucThietLap _ucThietLap;
        private ucBaoCao  _ucBaoCao;
        private Control   _currentUC;

        public frmMain(AccountDTO account)
        {
            CurrentAccount = account;
            InitializeComponent();
            SetupPermissions();
            // Mặc định mở Sơ đồ phòng
            NavigateTo(btnSoDo);
        }

        private void InitializeComponent()
        {
            this.Text            = "Quản Lý Khách Sạn";
            this.WindowState     = FormWindowState.Maximized;
            this.MinimumSize     = new Size(1100, 650);
            this.BackColor       = ContentBg;
            this.Font            = new Font("Segoe UI", 9.5f);

            // ── HEADER ────────────────────────────────────────
            pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 50,
                BackColor = HeaderColor
            };

            lblAppName = new Label
            {
                Text      = "  🏨  QUẢN LÝ KHÁCH SẠN",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize  = false,
                Height    = 50,
                Width     = 350,
                TextAlign = ContentAlignment.MiddleLeft,
                Location  = new Point(0, 0)
            };

            btnUserMenu = new Button
            {
                Text      = $"  👤  {CurrentAccount.DisplayName}  ▾",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10),
                Height    = 50,
                Width     = 200,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Right | AnchorStyles.Top
            };
            btnUserMenu.FlatAppearance.BorderSize = 0;
            btnUserMenu.Click += (s, e) =>
                menuUser.Show(btnUserMenu, new Point(0, btnUserMenu.Height));

            // Phải căn button về bên phải khi form resize
            pnlHeader.SizeChanged += (s, e) =>
                btnUserMenu.Location = new Point(pnlHeader.Width - 210, 0);
            btnUserMenu.Location = new Point(1200, 0);

            // Context menu user
            menuUser  = new ContextMenuStrip();
            mnuProfile = new ToolStripMenuItem("Thông tin cá nhân");
            // Thêm ngay sau dòng đó:
            mnuProfile.Click += (s, e) =>
            {
                var frm = new frmProfile(CurrentAccount);
                frm.ShowDialog();
                // Cập nhật lại tên hiển thị trên header nếu đã đổi
                btnUserMenu.Text = $"  👤  {CurrentAccount.DisplayName}  ▾";
            };
            mnuLogout  = new ToolStripMenuItem("Đăng xuất");
            mnuLogout.Click += MnuLogout_Click;
            menuUser.Items.AddRange(new ToolStripItem[] { mnuProfile, mnuLogout });

            pnlHeader.Controls.AddRange(new Control[] { lblAppName, btnUserMenu });

            // ── SIDEBAR ───────────────────────────────────────
            pnlSidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 200,
                BackColor = SidebarColor
            };

            // Logo/title khu vực trên sidebar
            var lblSideTitle = new Label
            {
                Text      = CurrentAccount.DisplayName.ToUpper(),
                ForeColor = Color.FromArgb(160, 160, 200),
                Font      = new Font("Segoe UI", 8),
                AutoSize  = false,
                Width     = 200,
                Height    = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(0, 10)
            };
            var lblRole = new Label
            {
                Text      = CurrentAccount.IsAdmin ? "● ADMIN" : "● NHÂN VIÊN",
                ForeColor = CurrentAccount.IsAdmin ? Color.FromArgb(100, 220, 120) : Color.FromArgb(100, 180, 255),
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                AutoSize  = false,
                Width     = 200,
                Height    = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(0, 40)
            };

            var divider = new Panel
            {
                BackColor = Color.FromArgb(70, 70, 100),
                Location  = new Point(15, 65),
                Size      = new Size(170, 1)
            };

            int btnTop = 80;
            btnSoDo     = CreateNavButton("🏠  Sơ đồ phòng",    ref btnTop);
            btnKhachHang = CreateNavButton("👥  Khách hàng",     ref btnTop);
            btnHoaDon   = CreateNavButton("🧾  Hóa đơn",         ref btnTop);
            btnThietLap = CreateNavButton("⚙️  Thiết lập",       ref btnTop);
            btnBaoCao   = CreateNavButton("📊  Báo cáo",         ref btnTop);

            btnSoDo.Click     += (s, e) => NavigateTo(btnSoDo);
            btnKhachHang.Click += (s, e) => NavigateTo(btnKhachHang);
            btnHoaDon.Click   += (s, e) => NavigateTo(btnHoaDon);
            btnThietLap.Click += (s, e) => NavigateTo(btnThietLap);
            btnBaoCao.Click   += (s, e) => NavigateTo(btnBaoCao);

            pnlSidebar.Controls.AddRange(new Control[]
            {
                lblSideTitle, lblRole, divider,
                btnSoDo, btnKhachHang, btnHoaDon, btnThietLap, btnBaoCao
            });

            // ── CONTENT ───────────────────────────────────────
            pnlContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = ContentBg,
                Padding   = new Padding(0)
            };

            this.Controls.AddRange(new Control[] { pnlContent, pnlSidebar, pnlHeader });
        }

        private Button CreateNavButton(string text, ref int top)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = new Point(0, top),
                Size      = new Size(200, 48),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(200, 200, 220),
                Font      = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize       = 0;
            btn.FlatAppearance.MouseOverBackColor = NavHover;
            top += 50;
            return btn;
        }

        private void SetupPermissions()
        {
            if (!CurrentAccount.IsAdmin)
            {
                btnThietLap.Enabled  = false;
                btnThietLap.ForeColor = Color.FromArgb(80, 80, 100);
                btnBaoCao.Enabled   = false;
                btnBaoCao.ForeColor  = Color.FromArgb(80, 80, 100);
            }
        }

        private void NavigateTo(Button btn)
        {
            // Reset nút cũ
            if (_activeButton != null)
            {
                _activeButton.BackColor = Color.Transparent;
                _activeButton.ForeColor = Color.FromArgb(200, 200, 220);
                _activeButton.Font      = new Font("Segoe UI", 10);
            }

            // Active nút mới
            btn.BackColor = NavActive;
            btn.ForeColor = Color.White;
            btn.Font      = new Font("Segoe UI", 10, FontStyle.Bold);
            _activeButton = btn;

            // Load UC tương ứng
            Control uc = null;
            if (btn == btnSoDo)
            {
                if (_ucSoDo == null) _ucSoDo = new ucSoDo(CurrentAccount);
                uc = _ucSoDo;
            }
            else if (btn == btnKhachHang)
            {
                if (_ucKhachHang == null) _ucKhachHang = new ucKhachHang();
                uc = _ucKhachHang;
            }
            else if (btn == btnHoaDon)
            {
                if (_ucHoaDon == null) _ucHoaDon = new ucHoaDon();
                uc = _ucHoaDon;
            }
            else if (btn == btnThietLap && CurrentAccount.IsAdmin)
            {
                if (_ucThietLap == null) _ucThietLap = new ucThietLap();
                uc = _ucThietLap;
            }
            else if (btn == btnBaoCao && CurrentAccount.IsAdmin)
            {
                if (_ucBaoCao == null) _ucBaoCao = new ucBaoCao();
                uc = _ucBaoCao;
            }

            if (uc == null) return;

            // Refresh nếu cần
            if (uc is ucSoDo s) s.LoadData();
            if (uc is ucKhachHang kh) kh.LoadData();
            if (uc is ucHoaDon hd) hd.LoadData();
            if (uc is ucBaoCao bc) bc.LoadAll(); // ← THÊM DÒNG NÀY

            ShowUserControl(uc);
            _currentUC = uc;
        }

        private void ShowUserControl(Control uc)
        {
            pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(uc);
        }

        private void MnuLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                var login = new frmLogin();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    CurrentAccount = login.LoggedInAccount;
                    // Restart với account mới
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}
