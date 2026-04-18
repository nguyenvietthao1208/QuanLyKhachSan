// =============================================
// UC - Thiết Lập (ucThietLap.cs)
// TabControl với 5 tab con
// =============================================

using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class ucThietLap : UserControl
    {
        private TabControl   tabMain;
        private TabPage      tabPhong, tabLoaiPhong, tabDichVu, tabTaiKhoan, tabKhachHang;

        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        public ucThietLap()
        {
            InitializeComponent();
            BuildTabPhong();
            BuildTabLoaiPhong();
            BuildTabDichVu();
            BuildTabTaiKhoan();
            BuildTabKhachHang();
        }

        private void InitializeComponent()
        {
            this.Dock      = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 248, 252);

            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 55,
                BackColor = Color.White
            };
            var lblTitle = new Label
            {
                Text      = "⚙️  Thiết Lập Hệ Thống",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize  = true,
                Location  = new Point(15, 13)
            };
            pnlHeader.Controls.Add(lblTitle);

            tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            tabPhong      = new TabPage("🏠  Phòng");
            tabLoaiPhong  = new TabPage("📋  Loại Phòng");
            tabDichVu     = new TabPage("🛎  Dịch Vụ");
            tabTaiKhoan   = new TabPage("👤  Tài Khoản");
            tabKhachHang  = new TabPage("👥  Khách Hàng");

            tabMain.TabPages.AddRange(new TabPage[]
                { tabPhong, tabLoaiPhong, tabDichVu, tabTaiKhoan, tabKhachHang });

            this.Controls.AddRange(new Control[] { tabMain, pnlHeader });
        }

        // ══════════════════════════════════════════════════════
        // TAB PHÒNG
        // ══════════════════════════════════════════════════════
        private DataGridView dgvRoom;
        private TextBox txtRoomNum, txtRoomFloor;
        private ComboBox cmbRoomCat;
        private Button btnRoomAdd, btnRoomEdit, btnRoomDel, btnRoomClear;
        private RoomBLL _roomBLL = new RoomBLL();
        private RoomCategoryBLL _catBLL = new RoomCategoryBLL();
        private int _editRoomId = 0;

        private void BuildTabPhong()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Form nhập
            var grp = new GroupBox
            {
                Text     = "Thông tin phòng",
                Location = new Point(10, 10),
                Size     = new Size(380, 185),
                Font     = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            int y = 28;
            grp.Controls.Add(MakeLbl("Số phòng:", 10, y)); txtRoomNum = MakeTxtBox(110, y, 150); grp.Controls.Add(txtRoomNum); y += 38;
            grp.Controls.Add(MakeLbl("Tầng:", 10, y)); txtRoomFloor = MakeTxtBox(110, y, 80); grp.Controls.Add(txtRoomFloor); y += 38;
            grp.Controls.Add(MakeLbl("Loại phòng:", 10, y));
            cmbRoomCat = new ComboBox { Location = new Point(110, y), Size = new Size(200, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            grp.Controls.Add(cmbRoomCat); y += 42;

            btnRoomAdd  = MakeActionBtn("+ Thêm",  Color.FromArgb(40, 167, 69),  new Point(10, y),  85);
            btnRoomEdit = MakeActionBtn("✏ Lưu",   PrimaryColor,                  new Point(103, y), 85);
            btnRoomDel  = MakeActionBtn("🗑 Xóa",  Color.FromArgb(220, 53, 69),  new Point(196, y), 85);
            btnRoomClear= MakeActionBtn("↺ Xóa trắng", Color.FromArgb(108,117,125), new Point(289, y), 85);

            btnRoomAdd.Click   += BtnRoomAdd_Click;
            btnRoomEdit.Click  += BtnRoomEdit_Click;
            btnRoomDel.Click   += BtnRoomDel_Click;
            btnRoomClear.Click += (s, e) => { _editRoomId = 0; txtRoomNum.Clear(); txtRoomFloor.Clear(); };

            grp.Controls.AddRange(new Control[] { btnRoomAdd, btnRoomEdit, btnRoomDel, btnRoomClear });

            dgvRoom = BuildDGV();
            dgvRoom.Location = new Point(10, 200);
            dgvRoom.Size = new Size(700, 300);
            dgvRoom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvRoom.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",         HeaderText="ID",            Width=50 },
                new DataGridViewTextBoxColumn { Name="RoomNumber", HeaderText="Số phòng",       Width=90 },
                new DataGridViewTextBoxColumn { Name="Floor",      HeaderText="Tầng",           Width=60 },
                new DataGridViewTextBoxColumn { Name="Category",   HeaderText="Loại phòng",     Width=150 },
                new DataGridViewTextBoxColumn { Name="Price",      HeaderText="Giá/đêm",        Width=120 },
                new DataGridViewTextBoxColumn { Name="Status",     HeaderText="Trạng thái",     Width=100, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });
            dgvRoom.SelectionChanged += DgvRoom_SelectionChanged;

            pnl.Controls.AddRange(new Control[] { grp, dgvRoom });
            tabPhong.Controls.Add(pnl);

            LoadRoomCategoryCombo();
            LoadRoomGrid();
        }

        private void LoadRoomCategoryCombo()
        {
            cmbRoomCat.DataSource    = _catBLL.GetAll();
            cmbRoomCat.DisplayMember = "Name";
            cmbRoomCat.ValueMember   = "Id";
        }

        private void LoadRoomGrid()
        {
            dgvRoom.Rows.Clear();
            foreach (var r in _roomBLL.GetAll())
                dgvRoom.Rows.Add(r.Id, r.RoomNumber, r.Floor, r.CategoryName, r.CategoryPrice.ToString("N0") + " ₫", r.Status);
        }

        private void DgvRoom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRoom.SelectedRows.Count == 0) return;
            var row = dgvRoom.SelectedRows[0];
            _editRoomId      = Convert.ToInt32(row.Cells["Id"].Value);
            txtRoomNum.Text  = row.Cells["RoomNumber"].Value.ToString();
            txtRoomFloor.Text = row.Cells["Floor"].Value.ToString();
        }

        private void BtnRoomAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtRoomFloor.Text, out int floor)) { MessageBox.Show("Số tầng không hợp lệ!"); return; }
            var room = new RoomDTO { RoomNumber = txtRoomNum.Text, Floor = floor, IdCategory = (int)(cmbRoomCat.SelectedValue ?? 1) };
            var (ok, msg) = _roomBLL.Insert(room);
            MessageBox.Show(msg); if (ok) LoadRoomGrid();
        }

        private void BtnRoomEdit_Click(object sender, EventArgs e)
        {
            if (_editRoomId == 0) { MessageBox.Show("Chọn phòng cần sửa!"); return; }
            if (!int.TryParse(txtRoomFloor.Text, out int floor)) { MessageBox.Show("Số tầng không hợp lệ!"); return; }
            var room = new RoomDTO { Id = _editRoomId, RoomNumber = txtRoomNum.Text, Floor = floor, IdCategory = (int)(cmbRoomCat.SelectedValue ?? 1) };
            var (ok, msg) = _roomBLL.Update(room);
            MessageBox.Show(msg); if (ok) { LoadRoomGrid(); _editRoomId = 0; }
        }

        private void BtnRoomDel_Click(object sender, EventArgs e)
        {
            if (_editRoomId == 0) { MessageBox.Show("Chọn phòng cần xóa!"); return; }
            if (MessageBox.Show("Xóa phòng này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var (ok, msg) = _roomBLL.Delete(_editRoomId);
                MessageBox.Show(msg); if (ok) { LoadRoomGrid(); _editRoomId = 0; }
            }
        }

        // ══════════════════════════════════════════════════════
        // TAB LOẠI PHÒNG
        // ══════════════════════════════════════════════════════
        private DataGridView dgvCat;
        private TextBox txtCatName, txtCatPrice;
        private Button btnCatAdd, btnCatEdit, btnCatDel;
        private int _editCatId = 0;

        private void BuildTabLoaiPhong()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var grp = new GroupBox { Text = "Loại phòng", Location = new Point(10, 10), Size = new Size(360, 145), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = PrimaryColor };

            int y = 28;
            grp.Controls.Add(MakeLbl("Tên loại:", 10, y)); txtCatName  = MakeTxtBox(110, y, 220); grp.Controls.Add(txtCatName); y += 38;
            grp.Controls.Add(MakeLbl("Giá/đêm:", 10, y));  txtCatPrice = MakeTxtBox(110, y, 150); grp.Controls.Add(txtCatPrice); y += 42;

            btnCatAdd  = MakeActionBtn("+ Thêm", Color.FromArgb(40, 167, 69),  new Point(10, y), 100);
            btnCatEdit = MakeActionBtn("✏ Lưu",  PrimaryColor,                  new Point(118, y), 100);
            btnCatDel  = MakeActionBtn("🗑 Xóa", Color.FromArgb(220, 53, 69),  new Point(226, y), 100);
            btnCatAdd.Click  += BtnCatAdd_Click;
            btnCatEdit.Click += BtnCatEdit_Click;
            btnCatDel.Click  += BtnCatDel_Click;
            grp.Controls.AddRange(new Control[] { btnCatAdd, btnCatEdit, btnCatDel });

            dgvCat = BuildDGV();
            dgvCat.Location = new Point(10, 165);
            dgvCat.Size = new Size(600, 300);
            dgvCat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvCat.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",    HeaderText="ID",        Width=50 },
                new DataGridViewTextBoxColumn { Name="Name",  HeaderText="Tên loại",  Width=200 },
                new DataGridViewTextBoxColumn { Name="Price", HeaderText="Giá/đêm",   Width=150, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });
            dgvCat.SelectionChanged += (s, e) =>
            {
                if (dgvCat.SelectedRows.Count == 0) return;
                _editCatId = Convert.ToInt32(dgvCat.SelectedRows[0].Cells["Id"].Value);
                txtCatName.Text  = dgvCat.SelectedRows[0].Cells["Name"].Value.ToString();
                txtCatPrice.Text = dgvCat.SelectedRows[0].Cells["Price"].Value.ToString().Replace(" ₫", "").Replace(",", "");
            };

            pnl.Controls.AddRange(new Control[] { grp, dgvCat });
            tabLoaiPhong.Controls.Add(pnl);
            LoadCatGrid();
        }

        private void LoadCatGrid()
        {
            dgvCat.Rows.Clear();
            foreach (var c in _catBLL.GetAll())
                dgvCat.Rows.Add(c.Id, c.Name, c.Price.ToString("N0") + " ₫");
        }

        private void BtnCatAdd_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtCatPrice.Text, out decimal price)) { MessageBox.Show("Giá không hợp lệ!"); return; }
            var cat = new RoomCategoryDTO { Name = txtCatName.Text, Price = price };
            var (ok, msg) = _catBLL.Insert(cat);
            MessageBox.Show(msg);
            if (ok)
            {
                LoadCatGrid();
                LoadRoomCategoryCombo(); // ← THÊM DÒNG NÀY
            }
        }

        private void BtnCatEdit_Click(object sender, EventArgs e)
        {
            if (_editCatId == 0) { MessageBox.Show("Chọn loại phòng cần sửa!"); return; }
            if (!decimal.TryParse(txtCatPrice.Text, out decimal price)) { MessageBox.Show("Giá không hợp lệ!"); return; }
            var cat = new RoomCategoryDTO { Id = _editCatId, Name = txtCatName.Text, Price = price };
            var (ok, msg) = _catBLL.Update(cat);
            MessageBox.Show(msg);
            if (ok)
            {
                LoadCatGrid();
                LoadRoomCategoryCombo(); // ← THÊM DÒNG NÀY
            }
        }

        private void BtnCatDel_Click(object sender, EventArgs e)
        {
            if (_editCatId == 0) { MessageBox.Show("Chọn loại phòng cần xóa!"); return; }
            if (MessageBox.Show("Xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var (ok, msg) = _catBLL.Delete(_editCatId);
                MessageBox.Show(msg);
                if (ok)
                {
                    LoadCatGrid();
                    LoadRoomCategoryCombo(); // ← THÊM DÒNG NÀY
                }
            }
        }

        // ══════════════════════════════════════════════════════
        // TAB DỊCH VỤ
        // ══════════════════════════════════════════════════════
        private DataGridView dgvSvc;
        private TextBox txtSvcName, txtSvcPrice;
        private CheckBox chkSvcActive;
        private Button btnSvcAdd, btnSvcEdit, btnSvcDel;
        private int _editSvcId = 0;
        private ServiceBLL _svcBLL = new ServiceBLL();

        private void BuildTabDichVu()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var grp = new GroupBox { Text = "Dịch vụ", Location = new Point(10, 10), Size = new Size(360, 180), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = PrimaryColor };

            int y = 28;
            grp.Controls.Add(MakeLbl("Tên dịch vụ:", 10, y)); txtSvcName  = MakeTxtBox(120, y, 210); grp.Controls.Add(txtSvcName); y += 38;
            grp.Controls.Add(MakeLbl("Giá:",          10, y)); txtSvcPrice = MakeTxtBox(120, y, 150); grp.Controls.Add(txtSvcPrice); y += 38;
            chkSvcActive = new CheckBox { Text = "Đang hoạt động", Location = new Point(10, y), AutoSize = true, Checked = true }; grp.Controls.Add(chkSvcActive); y += 34;

            btnSvcAdd  = MakeActionBtn("+ Thêm", Color.FromArgb(40, 167, 69),  new Point(10, y), 100);
            btnSvcEdit = MakeActionBtn("✏ Lưu",  PrimaryColor,                  new Point(118, y), 100);
            btnSvcDel  = MakeActionBtn("🗑 Xóa", Color.FromArgb(220, 53, 69),  new Point(226, y), 100);
            btnSvcAdd.Click  += BtnSvcAdd_Click;
            btnSvcEdit.Click += BtnSvcEdit_Click;
            btnSvcDel.Click  += BtnSvcDel_Click;
            grp.Controls.AddRange(new Control[] { btnSvcAdd, btnSvcEdit, btnSvcDel });

            dgvSvc = BuildDGV();
            dgvSvc.Location = new Point(10, 200);
            dgvSvc.Size = new Size(600, 300);
            dgvSvc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvSvc.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",     HeaderText="ID",          Width=50 },
                new DataGridViewTextBoxColumn { Name="Name",   HeaderText="Tên dịch vụ", Width=200 },
                new DataGridViewTextBoxColumn { Name="Price",  HeaderText="Giá",          Width=120 },
                new DataGridViewCheckBoxColumn { Name="Active", HeaderText="Hoạt động",   Width=90, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });
            dgvSvc.SelectionChanged += (s, e) =>
            {
                if (dgvSvc.SelectedRows.Count == 0) return;
                _editSvcId = Convert.ToInt32(dgvSvc.SelectedRows[0].Cells["Id"].Value);
                txtSvcName.Text  = dgvSvc.SelectedRows[0].Cells["Name"].Value.ToString();
                txtSvcPrice.Text = dgvSvc.SelectedRows[0].Cells["Price"].Value.ToString().Replace(" ₫", "").Replace(",", "");
                chkSvcActive.Checked = Convert.ToBoolean(dgvSvc.SelectedRows[0].Cells["Active"].Value);
            };

            pnl.Controls.AddRange(new Control[] { grp, dgvSvc });
            tabDichVu.Controls.Add(pnl);
            LoadSvcGrid();
        }

        private void LoadSvcGrid()
        {
            dgvSvc.Rows.Clear();
            foreach (var s in _svcBLL.GetAll())
                dgvSvc.Rows.Add(s.Id, s.ServiceName, s.Price.ToString("N0") + " ₫", s.IsActive);
        }

        private void BtnSvcAdd_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtSvcPrice.Text, out decimal price)) { MessageBox.Show("Giá không hợp lệ!"); return; }
            var svc = new ServiceDTO { ServiceName = txtSvcName.Text, Price = price, IsActive = chkSvcActive.Checked };
            var (ok, msg) = _svcBLL.Insert(svc); MessageBox.Show(msg); if (ok) LoadSvcGrid();
        }
        private void BtnSvcEdit_Click(object sender, EventArgs e)
        {
            if (_editSvcId == 0) { MessageBox.Show("Chọn dịch vụ cần sửa!"); return; }
            if (!decimal.TryParse(txtSvcPrice.Text, out decimal price)) { MessageBox.Show("Giá không hợp lệ!"); return; }
            var svc = new ServiceDTO { Id = _editSvcId, ServiceName = txtSvcName.Text, Price = price, IsActive = chkSvcActive.Checked };
            var (ok, msg) = _svcBLL.Update(svc); MessageBox.Show(msg); if (ok) LoadSvcGrid();
        }
        private void BtnSvcDel_Click(object sender, EventArgs e)
        {
            if (_editSvcId == 0) { MessageBox.Show("Chọn dịch vụ cần xóa!"); return; }
            if (MessageBox.Show("Xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { var (ok, msg) = _svcBLL.Delete(_editSvcId); MessageBox.Show(msg); if (ok) LoadSvcGrid(); }
        }

        // ══════════════════════════════════════════════════════
        // TAB TÀI KHOẢN
        // ══════════════════════════════════════════════════════
        private DataGridView dgvAcc;
        private TextBox txtAccUser, txtAccDisplay, txtAccPass;
        private ComboBox cmbAccRole;
        private Button btnAccAdd, btnAccEdit, btnAccDel;
        private int _editAccId = 0;
        private AccountBLL _accBLL = new AccountBLL();

        private void BuildTabTaiKhoan()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var grp = new GroupBox { Text = "Tài khoản", Location = new Point(10, 10), Size = new Size(400, 220), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = PrimaryColor };

            int y = 28;
            grp.Controls.Add(MakeLbl("Tên đăng nhập:", 10, y)); txtAccUser    = MakeTxtBox(145, y, 220); grp.Controls.Add(txtAccUser);    y += 38;
            grp.Controls.Add(MakeLbl("Họ tên:",         10, y)); txtAccDisplay = MakeTxtBox(145, y, 220); grp.Controls.Add(txtAccDisplay); y += 38;
            grp.Controls.Add(MakeLbl("Mật khẩu:",       10, y)); txtAccPass    = MakeTxtBox(145, y, 220); txtAccPass.UseSystemPasswordChar = true; grp.Controls.Add(txtAccPass); y += 38;
            grp.Controls.Add(MakeLbl("Quyền:",           10, y));
            cmbAccRole = new ComboBox { Location = new Point(145, y), Size = new Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbAccRole.Items.AddRange(new object[] { "admin", "staff" });
            cmbAccRole.SelectedIndex = 1;
            grp.Controls.Add(cmbAccRole); y += 42;

            btnAccAdd  = MakeActionBtn("+ Thêm", Color.FromArgb(40, 167, 69),  new Point(10, y), 100);
            btnAccEdit = MakeActionBtn("✏ Lưu",  PrimaryColor,                  new Point(118, y), 100);
            btnAccDel  = MakeActionBtn("🗑 Xóa", Color.FromArgb(220, 53, 69),  new Point(226, y), 100);
            btnAccAdd.Click  += BtnAccAdd_Click;
            btnAccEdit.Click += BtnAccEdit_Click;
            btnAccDel.Click  += BtnAccDel_Click;
            grp.Controls.AddRange(new Control[] { btnAccAdd, btnAccEdit, btnAccDel });

            dgvAcc = BuildDGV();
            dgvAcc.Location = new Point(10, 250);
            dgvAcc.Size = new Size(600, 280);
            dgvAcc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvAcc.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",      HeaderText="ID",             Width=50 },
                new DataGridViewTextBoxColumn { Name="User",    HeaderText="Tên đăng nhập",  Width=150 },
                new DataGridViewTextBoxColumn { Name="Display", HeaderText="Họ tên",          Width=180 },
                new DataGridViewTextBoxColumn { Name="Role",    HeaderText="Quyền",           Width=100, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });
            dgvAcc.SelectionChanged += (s, e) =>
            {
                if (dgvAcc.SelectedRows.Count == 0) return;
                _editAccId = Convert.ToInt32(dgvAcc.SelectedRows[0].Cells["Id"].Value);
                txtAccUser.Text    = dgvAcc.SelectedRows[0].Cells["User"].Value.ToString();
                txtAccDisplay.Text = dgvAcc.SelectedRows[0].Cells["Display"].Value.ToString();
                cmbAccRole.SelectedItem = dgvAcc.SelectedRows[0].Cells["Role"].Value.ToString();
            };

            pnl.Controls.AddRange(new Control[] { grp, dgvAcc });
            tabTaiKhoan.Controls.Add(pnl);
            LoadAccGrid();
        }

        private void LoadAccGrid()
        {
            dgvAcc.Rows.Clear();
            foreach (var a in _accBLL.GetAll())
                dgvAcc.Rows.Add(a.Id, a.UserName, a.DisplayName, a.Role);
        }

        private void BtnAccAdd_Click(object sender, EventArgs e)
        {
            var acc = new AccountDTO { UserName = txtAccUser.Text, DisplayName = txtAccDisplay.Text, PassWord = txtAccPass.Text, Role = cmbAccRole.SelectedItem?.ToString() ?? "staff" };
            var (ok, msg) = _accBLL.Insert(acc); MessageBox.Show(msg); if (ok) { LoadAccGrid(); txtAccPass.Clear(); }
        }
        private void BtnAccEdit_Click(object sender, EventArgs e)
        {
            if (_editAccId == 0) { MessageBox.Show("Chọn tài khoản cần sửa!"); return; }
            var acc = new AccountDTO { Id = _editAccId, UserName = txtAccUser.Text, DisplayName = txtAccDisplay.Text, PassWord = txtAccPass.Text, Role = cmbAccRole.SelectedItem?.ToString() ?? "staff" };
            var (ok, msg) = _accBLL.Update(acc); MessageBox.Show(msg); if (ok) LoadAccGrid();
        }
        private void BtnAccDel_Click(object sender, EventArgs e)
        {
            if (_editAccId == 0) { MessageBox.Show("Chọn tài khoản cần xóa!"); return; }
            if (MessageBox.Show("Xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // currentUserId = 0 (sẽ xử lý sau khi tích hợp account session)
                var (ok, msg) = _accBLL.Delete(_editAccId, 0);
                MessageBox.Show(msg); if (ok) LoadAccGrid();
            }
        }

        // ══════════════════════════════════════════════════════
        // TAB KHÁCH HÀNG (trong Thiết lập)
        // ══════════════════════════════════════════════════════
        private void BuildTabKhachHang()
        {
            var ucKH = new ucKhachHang();
            ucKH.Dock = DockStyle.Fill;
            ucKH.LoadData();
            tabKhachHang.Controls.Add(ucKH);
        }

        // ── Helpers ───────────────────────────────────────────
        private Label MakeLbl(string text, int x, int y) =>
            new Label { Text = text, Location = new Point(x, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) };

        private TextBox MakeTxtBox(int x, int y, int w) =>
            new TextBox { Location = new Point(x, y), Size = new Size(w, 28), Font = new Font("Segoe UI", 9) };

        private Button MakeActionBtn(string text, Color color, Point loc, int w)
        {
            var btn = new Button
            {
                Text = text, Location = loc, Size = new Size(w, 30),
                FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private DataGridView BuildDGV() => new DataGridView
        {
            ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.5f), RowHeadersVisible = false,
            ColumnHeadersHeight = 36, RowTemplate = { Height = 32 }
        };
    }
}
