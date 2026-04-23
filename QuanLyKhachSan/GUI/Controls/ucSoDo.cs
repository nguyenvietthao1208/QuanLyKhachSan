// =============================================
// UC - Sơ đồ Phòng (ucSoDo.cs)
// UserControl chính - hiển thị phòng phân tầng
// =============================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class ucSoDo : UserControl
    {
        // ── Layout ────────────────────────────────────────────
        private Panel      pnlLeft;        // Sơ đồ phòng (phân tầng)
        private Panel      pnlRight;       // Panel dịch vụ
        private SplitContainer split;
        public bool IsLoaded { get; private set; } = false;


        // Toolbar trên
        private Panel      pnlToolbar;
        private Label      lblTitle;
        private Label      lblTitle1;
        private Button     btnRefresh;
        private Label      lblLegend1;
        private Label      lblLegend2;

        // Right panel controls
        private Label      lblRoomInfo;
        private Label      lblCustInfo;
        private Label      lblServiceTitle;
        private ComboBox   cmbService;
        private NumericUpDown numQty;
        private Button     btnAddService;
        private DataGridView dgvServices;
        private Button     btnRemoveService;
        private Panel      pnlActions;
        private Button     btnCheckout;
        private Button     btnTransfer;
        private ComboBox   cmbTransferTo;
        private Label      lblTotal;
        private Label      lblTotalValue;

        // State
        private AccountDTO _account;
        private RoomDTO    _selectedRoom;
        private BookingDTO _activeBooking;

        private readonly RoomBLL     _roomBLL    = new RoomBLL();
        private readonly BookingBLL  _bookingBLL = new BookingBLL();
        private readonly ServiceBLL  _serviceBLL = new ServiceBLL();

        // Colors
        private readonly Color ColorEmpty    = Color.FromArgb(100, 210, 130);  // Xanh lá - Trống
        private readonly Color ColorOccupied = Color.FromArgb(220, 80, 80);    // Đỏ - Có người
        private readonly Color ColorSelected = Color.FromArgb(255, 180, 50);   // Vàng - Đang chọn
        private readonly Color PrimaryColor  = Color.FromArgb(106, 90, 205);
        private readonly Color BgColor       = Color.FromArgb(248, 248, 252);

        public ucSoDo(AccountDTO account)
        {
            _account = account;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock      = DockStyle.Fill;
            this.BackColor = BgColor;

            // ── TOOLBAR ───────────────────────────────────────
            pnlToolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 55,
                BackColor = Color.White,
                Padding   = new Padding(15, 0, 15, 0)
            };
            pnlToolbar.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 220, 230)),
                    0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            lblTitle = new Label
            {
                Text      = "🏠  Sơ Đồ Phòng",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize  = true,
                Location  = new Point(15, 13)
            };

            lblTitle1 = new Label
            {
                Text      = "Trạng thái:",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize  = true,
                Location  = new Point(430, 13)
            };

            btnRefresh = new Button
            {
                Text      = "🔄 Làm mới",
                Location  = new Point(300, 13),
                Size      = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadData();

            // Legend
            lblLegend1 = MakeLegend("  Trống  ", ColorEmpty, new Point(540, 16));
            lblLegend2 = MakeLegend("  Có người  ", ColorOccupied, new Point(640, 16));

            pnlToolbar.Controls.AddRange(new Control[] { lblTitle, lblTitle1, btnRefresh, lblLegend1, lblLegend2 });

            // ── SPLIT CONTAINER ───────────────────────────────
            split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BorderStyle = BorderStyle.None,
                BackColor = BgColor
            };

            // Left = sơ đồ phòng (scrollable)
            pnlLeft = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = BgColor,
                Padding = new Padding(15)
            };
            split.Panel1.Controls.Add(pnlLeft);

            // Right = panel dịch vụ
            BuildRightPanel();
            split.Panel2.Controls.Add(pnlRight);

            this.Controls.AddRange(new Control[] { split, pnlToolbar });

            // Tất cả min size và splitter distance set SAU KHI load
            this.Load += (s, e) =>
            {
                split.Panel1MinSize = 400;
                split.Panel2MinSize = 350;
                if (split.Width > 760)
                    split.SplitterDistance = split.Width - 420;

                IsLoaded = true;
                LoadData();
            };
        }

        private Label MakeLegend(string text, Color color, Point loc)
        {
            var lbl = new Label
            {
                Text      = text,
                BackColor = color,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize  = false,
                Size      = new Size(90, 24),
                Location  = loc,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lbl.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, lbl.Width, lbl.Height, 8, 8));
            return lbl;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void BuildRightPanel()
        {
            pnlRight = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
                Padding   = new Padding(15)
            };

            lblRoomInfo = new Label
            {
                Text      = "← Chọn phòng để xem thông tin",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location  = new Point(15, 15),
                AutoSize  = false,
                Size      = new Size(350, 28)
            };

            lblCustInfo = new Label
            {
                Text      = "",
                Font      = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location  = new Point(15, 48),
                AutoSize  = false,
                Size      = new Size(350, 60)
            };

            // Separator
            var sep1 = new Panel { BackColor = Color.FromArgb(220,220,230), Location = new Point(15, 113), Size = new Size(350, 1) };

            lblServiceTitle = new Label
            {
                Text      = "DỊCH VỤ ĐÃ SỬ DỤNG",
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 120, 140),
                Location  = new Point(15, 122),
                AutoSize  = true
            };

            // Add service row
            var lblSvcLabel = new Label
            {
                Text     = "Dịch vụ:",
                Location = new Point(15, 152),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9)
            };

            cmbService = new ComboBox
            {
                Location      = new Point(70, 148),
                Size          = new Size(170, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9)
            };

            numQty = new NumericUpDown
            {
                Location  = new Point(248, 148),
                Size      = new Size(50, 28),
                Minimum   = 1,
                Maximum   = 99,
                Value     = 1,
                Font      = new Font("Segoe UI", 9)
            };

            btnAddService = new Button
            {
                Text      = "+ Thêm",
                Location  = new Point(305, 148),
                Size      = new Size(65, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnAddService.FlatAppearance.BorderSize = 0;
            btnAddService.Click += BtnAddService_Click;

            // DataGridView dịch vụ
            dgvServices = new DataGridView
            {
                Location          = new Point(15, 185),
                Size              = new Size(355, 220),
                ReadOnly          = true,
                AllowUserToAddRows = false,
                SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor   = Color.White,
                BorderStyle       = BorderStyle.FixedSingle,
                Font              = new Font("Segoe UI", 9),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvServices.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="id",      HeaderText="ID",          Visible=false },
                new DataGridViewTextBoxColumn { Name="name",    HeaderText="Tên dịch vụ", FillWeight=50 },
                new DataGridViewTextBoxColumn { Name="qty",     HeaderText="SL",          FillWeight=15 },
                new DataGridViewTextBoxColumn { Name="price",   HeaderText="Đơn giá",     FillWeight=25 },
                new DataGridViewTextBoxColumn { Name="total",   HeaderText="Thành tiền",  FillWeight=25 }
            });

            btnRemoveService = new Button
            {
                Text      = "🗑 Xóa dịch vụ",
                Location  = new Point(15, 412),
                Size      = new Size(130, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };
            btnRemoveService.FlatAppearance.BorderSize = 0;
            btnRemoveService.Click += BtnRemoveService_Click;

            // Tổng tiền
            var sep2 = new Panel { BackColor = Color.FromArgb(220,220,230), Location = new Point(15, 450), Size = new Size(350, 1) };

            lblTotal = new Label
            {
                Text      = "TỔNG TIỀN:",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location  = new Point(15, 460),
                AutoSize  = true
            };

            lblTotalValue = new Label
            {
                Text      = "0 VNĐ",
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Location  = new Point(200, 458),
                AutoSize  = true
            };

            // Actions
            pnlActions = new Panel
            {
                Location  = new Point(15, 500),
                Size      = new Size(355, 200),
                BackColor = Color.White
            };

            btnCheckout = new Button
            {
                Text      = "💳  Thanh Toán",
                Location  = new Point(0, 0),
                Size      = new Size(170, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += BtnCheckout_Click;

            btnTransfer = new Button
            {
                Text      = "↔  Chuyển Phòng",
                Location  = new Point(0, 50),
                Size      = new Size(170, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnTransfer.FlatAppearance.BorderSize = 0;
            btnTransfer.Click += BtnTransfer_Click;

            cmbTransferTo = new ComboBox
            {
                Location      = new Point(180, 50),
                Size          = new Size(170, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9),
                Visible       = false
            };

            pnlActions.Controls.AddRange(new Control[] { btnCheckout, btnTransfer, cmbTransferTo });

            // Mặc định disable
            SetRightPanelEnabled(false);

            pnlRight.Controls.AddRange(new Control[]
            {
                lblRoomInfo, lblCustInfo, sep1, lblServiceTitle,
                lblSvcLabel, cmbService, numQty, btnAddService,
                dgvServices, btnRemoveService, sep2,
                lblTotal, lblTotalValue, pnlActions
            });
        }

        // ══════════════════════════════════════════════════════
        // LOAD DATA
        // ══════════════════════════════════════════════════════
        public void LoadData()
        {
            pnlLeft.Controls.Clear();
            _selectedRoom  = null;
            _activeBooking = null;
            SetRightPanelEnabled(false);
            lblRoomInfo.Text = "← Chọn phòng để xem thông tin";
            lblCustInfo.Text = "";
            dgvServices.Rows.Clear();
            lblTotalValue.Text = "0 VNĐ";

            var rooms  = _roomBLL.GetAll();
            var floors = rooms.Select(r => r.Floor).Distinct().OrderBy(f => f).ToList();

            int yOffset = 10;
            foreach (int floor in floors)
            {
                // Tiêu đề tầng
                var lblFloor = new Label
                {
                    Text      = $"  TẦNG {floor}",
                    Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 100, 130),
                    Location  = new Point(5, yOffset),
                    AutoSize  = false,
                    Size      = new Size(600, 28),
                    BackColor = Color.FromArgb(235, 235, 245)
                };
                pnlLeft.Controls.Add(lblFloor);
                yOffset += 35;

                // Vẽ các phòng trong tầng
                int xOffset = 10;
                var floorRooms = rooms.Where(r => r.Floor == floor).ToList();

                foreach (var room in floorRooms)
                {
                    var btn = CreateRoomButton(room);
                    btn.Location = new Point(xOffset, yOffset);
                    pnlLeft.Controls.Add(btn);
                    xOffset += 115;

                    if (xOffset > pnlLeft.Width - 120)
                    {
                        xOffset  = 10;
                        yOffset += 110;
                    }
                }
                yOffset += 110;
            }

            // Load dịch vụ vào combobox
            LoadServicesCombo();
        }

        private Button CreateRoomButton(RoomDTO room)
        {
            bool occupied = !room.IsEmpty;
            var btn = new Button
            {
                Tag       = room,
                Size      = new Size(105, 95),
                FlatStyle = FlatStyle.Flat,
                BackColor = occupied ? ColorOccupied : ColorEmpty,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Text      = $"Phòng {room.RoomNumber}\n{(occupied ? "Có người" : "Trống")}\n{room.CategoryName}",
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;

            // Rounded corners
            btn.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 12, 12));

            if (occupied)
            {
                // Click trái: chọn phòng để xem dịch vụ
                btn.Click += (s, e) => SelectOccupiedRoom(room, btn);
            }
            else
            {
                // Click phải: menu đặt phòng
                var ctxMenu = new ContextMenuStrip();
                var mnuBook = new ToolStripMenuItem($"📋  Đặt phòng {room.RoomNumber}");
                mnuBook.Click += (s, e) => OpenBookingForm(room);
                ctxMenu.Items.Add(mnuBook);
                btn.ContextMenuStrip = ctxMenu;

                // Click trái cũng hiển thị tooltip nhắc nhở
                btn.Click += (s, e) =>
                {
                    ToolTip tt = new ToolTip();
                    tt.Show("Click chuột PHẢI để đặt phòng", btn, 2000);
                };
            }

            return btn;
        }

        private void SelectOccupiedRoom(RoomDTO room, Button btn)
        {
            // Reset màu button cũ
            foreach (Control c in pnlLeft.Controls)
                if (c is Button b && b.Tag is RoomDTO r && !r.IsEmpty)
                    b.BackColor = ColorOccupied;

            btn.BackColor = ColorSelected;
            _selectedRoom = room;

            // Lấy booking hiện tại
            _activeBooking = _bookingBLL.GetActiveByRoomId(room.Id);
            if (_activeBooking == null)
            {
                MessageBox.Show("Không tìm thấy thông tin đặt phòng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị thông tin
            lblRoomInfo.Text = $"🏠 Phòng {room.RoomNumber}  |  {room.CategoryName}";
            lblCustInfo.Text =
                $"👤 {_activeBooking.CustomerName}  |  📞 {_activeBooking.CustomerPhone}\n" +
                $"🪪 CCCD: {_activeBooking.CustomerIdCard}\n" +
                $"📅 Check-in: {_activeBooking.CheckIn:dd/MM/yyyy}  →  Trả: {_activeBooking.CheckOut:dd/MM/yyyy}  ({_activeBooking.Nights} đêm)";

            SetRightPanelEnabled(true);
            LoadServiceGrid();
            UpdateTotal();
            LoadTransferRooms();
        }

        private void LoadServiceGrid()
        {
            dgvServices.Rows.Clear();
            if (_activeBooking == null) return;

            var services = _bookingBLL.GetServices(_activeBooking.Id);
            foreach (var svc in services)
            {
                dgvServices.Rows.Add(
                    svc.Id,
                    svc.ServiceName,
                    svc.Count,
                    svc.UnitPrice.ToString("N0") + " ₫",
                    svc.Total.ToString("N0") + " ₫"
                );
            }
        }

        private void LoadServicesCombo()
        {
            cmbService.DataSource    = null;
            cmbService.DataSource    = _serviceBLL.GetActive();
            cmbService.DisplayMember = "ServiceName";
            cmbService.ValueMember   = "Id";
        }

        private void LoadTransferRooms()
        {
            cmbTransferTo.Items.Clear();
            var rooms = _roomBLL.GetAll().Where(r => r.IsEmpty && r.Id != _selectedRoom?.Id).ToList();
            foreach (var r in rooms)
                cmbTransferTo.Items.Add(r);
            cmbTransferTo.DisplayMember = "RoomNumber";
        }

        private void UpdateTotal()
        {
            if (_activeBooking == null) return;
            decimal svcTotal  = _bookingBLL.GetServices(_activeBooking.Id).Sum(s => s.Total);
            decimal roomTotal = _activeBooking.RoomAmount;
            decimal grand     = roomTotal + svcTotal;

            lblTotalValue.Text =
                $"Phòng: {roomTotal:N0} ₫\n" +
                $"DV: {svcTotal:N0} ₫\n" +
                $"Tổng: {grand:N0} ₫";
        }

        private void SetRightPanelEnabled(bool enabled)
        {
            cmbService.Enabled      = enabled;
            numQty.Enabled          = enabled;
            btnAddService.Enabled   = enabled;
            btnRemoveService.Enabled = enabled;
            btnCheckout.Enabled     = enabled;
            btnTransfer.Enabled     = enabled;
            cmbTransferTo.Enabled   = enabled;
        }

        // ── EVENT HANDLERS ────────────────────────────────────

        private void BtnAddService_Click(object sender, EventArgs e)
        {
            if (_activeBooking == null || cmbService.SelectedItem == null) return;

            var svc   = (ServiceDTO)cmbService.SelectedItem;
            int count = (int)numQty.Value;

            var (ok, msg) = _bookingBLL.AddService(
                _activeBooking.Id, svc.Id, svc.ServiceName, svc.Price, count);

            if (ok)
            {
                LoadServiceGrid();
                UpdateTotal();
                numQty.Value = 1;
            }
            else
            {
                MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRemoveService_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0 || _activeBooking == null) return;

            int bsId = Convert.ToInt32(dgvServices.SelectedRows[0].Cells["id"].Value);
            if (MessageBox.Show("Xóa dịch vụ này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool ok = _bookingBLL.RemoveService(bsId, _activeBooking.Id);
                if (ok) { LoadServiceGrid(); UpdateTotal(); }
            }
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (_activeBooking == null || _selectedRoom == null) return;

            // Tính lại tổng
            decimal svcTotal  = _bookingBLL.GetServices(_activeBooking.Id).Sum(s => s.Total);
            decimal roomTotal = _activeBooking.RoomAmount;
            decimal grand     = roomTotal + svcTotal;

            var confirm = MessageBox.Show(
                $"Xác nhận thanh toán phòng {_selectedRoom.RoomNumber}?\n\n" +
                $"Tiền phòng:  {roomTotal:N0} ₫\n" +
                $"Tiền dịch vụ: {svcTotal:N0} ₫\n" +
                $"──────────────────────\n" +
                $"TỔNG CỘNG: {grand:N0} ₫",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (ok, msg) = _bookingBLL.Checkout(_activeBooking.Id, _selectedRoom.Id);
                MessageBox.Show(msg, ok ? "Thành công" : "Lỗi",
                    MessageBoxButtons.OK,
                    ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (ok) LoadData();
            }
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            if (_activeBooking == null || _selectedRoom == null) return;

            cmbTransferTo.Visible = !cmbTransferTo.Visible;

            if (cmbTransferTo.Visible && cmbTransferTo.SelectedItem != null)
            {
                var targetRoom = (RoomDTO)cmbTransferTo.SelectedItem;
                var confirm = MessageBox.Show(
                    $"Chuyển từ Phòng {_selectedRoom.RoomNumber} → Phòng {targetRoom.RoomNumber}?",
                    "Xác nhận chuyển phòng",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    var (ok, msg) = _bookingBLL.TransferRoom(
                        _activeBooking.Id, _selectedRoom.Id, targetRoom.Id, targetRoom.Status);
                    MessageBox.Show(msg, ok ? "Thành công" : "Lỗi",
                        MessageBoxButtons.OK,
                        ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                    if (ok) LoadData();
                }
            }
        }

        private void OpenBookingForm(RoomDTO room)
        {
            var frm = new frmDatPhong(room);
            if (frm.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }
}
