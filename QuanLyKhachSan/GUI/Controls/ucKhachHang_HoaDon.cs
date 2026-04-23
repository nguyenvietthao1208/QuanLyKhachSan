// =============================================
// UC - Khách Hàng (ucKhachHang.cs)
// =============================================

using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace QuanLyKhachSan.GUI
{
    // ══════════════════════════════════════════════════════════
    // KHÁCH HÀNG UC
    // ══════════════════════════════════════════════════════════
    public class ucKhachHang : UserControl
    {
        private DataGridView dgv;
        private TextBox      txtSearch;
        private Button       btnSearch, btnAdd, btnEdit, btnDelete, btnRefresh;
        private Label        lblTitle;
        private Panel        pnlTop;
        
        private readonly CustomerBLL _bll = new CustomerBLL();
        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        public ucKhachHang() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Dock      = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 248, 252);

            pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.White,
                Padding   = new Padding(15, 0, 15, 0)
            };

            lblTitle = new Label
            {
                Text      = "👥  Quản Lý Khách Hàng",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize  = true,
                Location  = new Point(15, 15)
            };

            txtSearch = new TextBox
            {
                Text = "Tìm theo tên, CCCD, SĐT...",
                Location        = new Point(280, 18),
                Size            = new Size(220, 28),
                Font            = new Font("Segoe UI", 9)
            };

            btnSearch = MakeBtn("🔍 Tìm", PrimaryColor, new Point(508, 18), 80);
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text);

            btnAdd = MakeBtn("+ Thêm", Color.FromArgb(40, 167, 69), new Point(600, 18), 80);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = MakeBtn("✏ Sửa", Color.FromArgb(23, 162, 184), new Point(688, 18), 80);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = MakeBtn("🗑 Xóa", Color.FromArgb(220, 53, 69), new Point(776, 18), 80);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = MakeBtn("🔄", Color.FromArgb(108, 117, 125), new Point(864, 18), 40);
            btnRefresh.Click += (s, e) => LoadData();           

            pnlTop.Controls.AddRange(new Control[] { lblTitle, txtSearch, btnSearch, btnAdd, btnEdit, btnDelete, btnRefresh });

            dgv = BuildDGV();
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",       HeaderText="ID",              Width=50 },
                new DataGridViewTextBoxColumn { Name="FullName", HeaderText="Họ và tên",       Width=180 },
                new DataGridViewTextBoxColumn { Name="IdCard",   HeaderText="CCCD/CMND",       Width=140 },
                new DataGridViewTextBoxColumn { Name="Phone",    HeaderText="Số điện thoại",   Width=120 },
                new DataGridViewTextBoxColumn { Name="Address",  HeaderText="Địa chỉ",         Width=180, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewTextBoxColumn { Name="Email",    HeaderText="Email",            Width=160 }
            });

            this.Controls.AddRange(new Control[] { dgv, pnlTop });
        }

        public void LoadData(string search = "")
        {
            dgv.Rows.Clear();
            List<CustomerDTO> list = string.IsNullOrWhiteSpace(search)
                ? _bll.GetAll() : _bll.Search(search);

            foreach (var c in list)
                dgv.Rows.Add(c.Id, c.FullName, c.IdCard, c.Phone, c.Address, c.Email);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var frm = new frmCustomerDetail(null);
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Vui lòng chọn khách hàng!"); return; }
            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            var cust = _bll.GetById(id);
            var frm = new frmCustomerDetail(cust);
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Vui lòng chọn khách hàng!"); return; }
            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var (ok, msg) = _bll.Delete(id);
                MessageBox.Show(msg);
                if (ok) LoadData();
            }
        }

        private Button MakeBtn(string text, Color color, Point loc, int width)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = loc,
                Size      = new Size(width, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 8.5f),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private DataGridView BuildDGV()
        {
            return new DataGridView
            {
                Dock              = DockStyle.Fill,
                ReadOnly          = true,
                AllowUserToAddRows = false,
                SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor   = Color.White,
                BorderStyle       = BorderStyle.None,
                Font              = new Font("Segoe UI", 9.5f),
                RowHeadersVisible = false,
                ColumnHeadersHeight = 38,
                RowTemplate       = { Height = 34 }
            };
        }
    }

    // ══════════════════════════════════════════════════════════
    // FORM THÊM/SỬA KHÁCH HÀNG
    // ══════════════════════════════════════════════════════════
    public class frmCustomerDetail : Form
    {
        private CustomerDTO _customer;
        private readonly CustomerBLL _bll = new CustomerBLL();

        private TextBox txtName, txtIdCard, txtPhone, txtAddress, txtEmail;
        private Button btnSave, btnCancel;
        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        public frmCustomerDetail(CustomerDTO customer)
        {
            _customer = customer;
            InitializeComponent();
            if (_customer != null) FillForm();
        }

        private void InitializeComponent()
        {
            this.Text            = _customer == null ? "Thêm Khách Hàng" : "Sửa Khách Hàng";
            this.Size            = new Size(430, 430);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.BackColor       = Color.FromArgb(248, 248, 252);
            this.Font            = new Font("Segoe UI", 9.5f);

            int y = 20;
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            AddField(pnl, "Họ và tên *",       ref y, out txtName);
            AddField(pnl, "CCCD/CMND *",        ref y, out txtIdCard);
            AddField(pnl, "Số điện thoại *",    ref y, out txtPhone);
            AddField(pnl, "Địa chỉ",            ref y, out txtAddress);
            AddField(pnl, "Email",               ref y, out txtEmail);

            btnSave = new Button
            {
                Text      = "💾  Lưu",
                Location  = new Point(70, y + 10),
                Size      = new Size(160, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text         = "Hủy",
                Location     = new Point(240, y + 10),
                Size         = new Size(100, 38),
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(108, 117, 125),
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 10),
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            pnl.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.Controls.Add(pnl);
        }

        private void AddField(Panel pnl, string label, ref int y, out TextBox txt)
        {
            pnl.Controls.Add(new Label { Text = label, Location = new Point(35, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            y += 22;
            txt = new TextBox { Location = new Point(35, y), Size = new Size(340, 28), Font = new Font("Segoe UI", 9) };
            pnl.Controls.Add(txt);
            y += 38;
        }

        private void FillForm()
        {
            txtName.Text    = _customer.FullName;
            txtIdCard.Text  = _customer.IdCard;
            txtPhone.Text   = _customer.Phone;
            txtAddress.Text = _customer.Address;
            txtEmail.Text   = _customer.Email;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var cust = new CustomerDTO
            {
                Id       = _customer?.Id ?? 0,
                FullName = txtName.Text.Trim(),
                IdCard   = txtIdCard.Text.Trim(),
                Phone    = txtPhone.Text.Trim(),
                Address  = txtAddress.Text.Trim(),
                Email    = txtEmail.Text.Trim()
            };

            bool ok; string msg;
            if (_customer == null)
            {
                var (s, m, _) = _bll.Insert(cust);
                ok = s; msg = m;
            }
            else
            {
                (ok, msg) = _bll.Update(cust);
            }

            MessageBox.Show(msg, ok ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (ok) this.DialogResult = DialogResult.OK;
        }
    }

    // ══════════════════════════════════════════════════════════
    // HÓA ĐƠN UC
    // ══════════════════════════════════════════════════════════
    public class ucHoaDon : UserControl
    {
        private DataGridView dgv;
        private Panel        pnlTop;
        private Label        lblTitle;
        private Button       btnRefresh;
        private Label        lblFilter;
        private ComboBox     cmbFilter;
        private Button       btnPrintBill;

        private readonly BookingBLL _bll = new BookingBLL();
        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        public ucHoaDon() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Dock      = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 248, 252);

            pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text      = "🧾  Hóa Đơn",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize  = true,
                Location  = new Point(15, 15)
            };

            lblFilter = new Label { Text = "Lọc:", Location = new Point(280, 20), AutoSize = true };
            cmbFilter = new ComboBox
            {
                Location      = new Point(310, 17),
                Size          = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9)
            };
            cmbFilter.Items.AddRange(new object[] { "Tất cả", "Đang ở", "Đã trả" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadData();

            btnRefresh = new Button
            {
                Text      = "🔄 Làm mới",
                Location  = new Point(475, 17),
                Size      = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadData();

            btnPrintBill = new Button
            {
                Text = "🖨 In Bill",
                Location = new Point(585, 17),
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnPrintBill.FlatAppearance.BorderSize = 0;
            btnPrintBill.Click += BtnPrintBill_Click;

            pnlTop.Controls.AddRange(new Control[] { lblTitle, lblFilter, cmbFilter, btnRefresh, btnPrintBill });

            dgv = new DataGridView
            {
                Dock              = DockStyle.Fill,
                ReadOnly          = true,
                AllowUserToAddRows = false,
                SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor   = Color.White,
                BorderStyle       = BorderStyle.None,
                Font              = new Font("Segoe UI", 9.5f),
                RowHeadersVisible = false,
                ColumnHeadersHeight = 38,
                RowTemplate       = { Height = 34 }
            };

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",          HeaderText="Mã HĐ",     Width=65 },
                new DataGridViewTextBoxColumn { Name="Room",        HeaderText="Phòng",      Width=80 },
                new DataGridViewTextBoxColumn { Name="Customer",    HeaderText="Khách hàng", Width=160 },
                new DataGridViewTextBoxColumn { Name="CheckIn",     HeaderText="Check-in",   Width=100 },
                new DataGridViewTextBoxColumn { Name="CheckOut",    HeaderText="Check-out",  Width=100 },
                new DataGridViewTextBoxColumn { Name="RoomAmt",     HeaderText="Tiền phòng", Width=110 },
                new DataGridViewTextBoxColumn { Name="SvcAmt",      HeaderText="Tiền DV",    Width=110 },
                new DataGridViewTextBoxColumn { Name="Total",       HeaderText="Tổng tiền",  Width=120, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewTextBoxColumn { Name="Status",      HeaderText="Trạng thái",Width=100 }
            });

            dgv.CellFormatting += Dgv_CellFormatting;

            this.Controls.AddRange(new Control[] { dgv, pnlTop });
        }

        public void LoadData()
        {
            dgv.Rows.Clear();
            var list = _bll.GetAll();
            string filter = cmbFilter.SelectedItem?.ToString();

            foreach (var b in list)
            {
                if (filter != "Tất cả" && b.Status != filter) continue;
                dgv.Rows.Add(
                    b.Id,
                    b.RoomNumber,
                    b.CustomerName,
                    b.CheckIn.ToString("dd/MM/yyyy"),
                    b.CheckOut.ToString("dd/MM/yyyy"),
                    b.RoomAmount.ToString("N0") + " ₫",
                    b.ServiceAmount.ToString("N0") + " ₫",
                    b.TotalPrice.ToString("N0") + " ₫",
                    b.Status
                );
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.CellStyle.ForeColor = e.Value.ToString() == "Đang ở"
                    ? Color.FromArgb(220, 53, 69)
                    : Color.FromArgb(108, 117, 125);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }
        private void BtnPrintBill_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần in!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int bookingId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            var allBookings = _bll.GetAll();
            var booking = allBookings.Find(b => b.Id == bookingId);
            if (booking == null) return;

            var services = _bll.GetBookingServices(bookingId);

            var pd = new PrintDocument();
            pd.PrintPage += (s, pe) => DrawBill(pe, booking, services);

            var preview = new PrintPreviewDialog
            {
                Document = pd,
                WindowState = FormWindowState.Maximized,
                Text = $"Xem trước hóa đơn - Phòng {booking.RoomNumber}"
            };
            preview.ShowDialog();
        }

        private void DrawBill(PrintPageEventArgs e, BookingDTO booking,
            List<BookingServiceDTO> services)
        {
            var g = e.Graphics;
            var fntTitle = new Font("Arial", 16, FontStyle.Bold);
            var fntSub = new Font("Arial", 11, FontStyle.Bold);
            var fntNormal = new Font("Arial", 10);
            var fntSmall = new Font("Arial", 9);
            var fntBold = new Font("Arial", 10, FontStyle.Bold);
            var fntTotal = new Font("Arial", 13, FontStyle.Bold);
            var brPurple = new SolidBrush(Color.FromArgb(106, 90, 205));
            var brGray = new SolidBrush(Color.FromArgb(240, 240, 248));
            var brRed = new SolidBrush(Color.FromArgb(220, 53, 69));

            float pageW = e.PageBounds.Width;
            float margin = 60;
            float y = 40;

            // Header
            g.FillRectangle(brPurple, margin, y, pageW - margin * 2, 55);
            g.DrawString("QUẢN LÝ KHÁCH SẠN",
                new Font("Arial", 18, FontStyle.Bold), Brushes.White, margin + 20, y + 10);
            y += 70;

            // Tiêu đề
            var titleSize = g.MeasureString("HÓA ĐƠN THANH TOÁN", fntTitle);
            g.DrawString("HÓA ĐƠN THANH TOÁN", fntTitle, brPurple,
                (pageW - titleSize.Width) / 2, y);
            y += titleSize.Height + 5;

            var subStr = $"Mã HĐ: #{booking.Id:D4}    |    Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}";
            var subSize = g.MeasureString(subStr, fntSmall);
            g.DrawString(subStr, fntSmall, Brushes.Gray, (pageW - subSize.Width) / 2, y);
            y += subSize.Height + 15;

            g.DrawLine(new Pen(Color.FromArgb(106, 90, 205), 2), margin, y, pageW - margin, y);
            y += 15;

            // Thông tin khách & phòng (2 cột)
            float col1 = margin, col2 = pageW / 2 + 20;
            g.DrawString("THÔNG TIN KHÁCH HÀNG", fntSub, brPurple, col1, y);
            g.DrawString("THÔNG TIN PHÒNG", fntSub, brPurple, col2, y);
            y += 25;

            float yL = y, yR = y;
            DrawInfoRow(g, fntBold, fntNormal, col1, ref yL, "Họ tên:", booking.CustomerName);
            DrawInfoRow(g, fntBold, fntNormal, col1, ref yL, "CCCD:", booking.CustomerIdCard);
            DrawInfoRow(g, fntBold, fntNormal, col1, ref yL, "SĐT:", booking.CustomerPhone);
            DrawInfoRow(g, fntBold, fntNormal, col2, ref yR, "Số phòng:", booking.RoomNumber);
            DrawInfoRow(g, fntBold, fntNormal, col2, ref yR, "Check-in:", booking.CheckIn.ToString("dd/MM/yyyy"));
            DrawInfoRow(g, fntBold, fntNormal, col2, ref yR, "Check-out:", booking.CheckOut.ToString("dd/MM/yyyy"));
            DrawInfoRow(g, fntBold, fntNormal, col2, ref yR, "Số đêm:", $"{booking.Nights} đêm");

            y = Math.Max(yL, yR) + 15;
            g.DrawLine(new Pen(Color.FromArgb(220, 220, 230)), margin, y, pageW - margin, y);
            y += 15;

            // Bảng chi tiết
            g.DrawString("CHI TIẾT HÓA ĐƠN", fntSub, brPurple, margin, y);
            y += 28;

            float[] colX = { margin, margin + 40, margin + 290, margin + 350, margin + 460 };
            float[] colW = { 40, 250, 60, 110, 110 };
            string[] headers = { "STT", "Nội dung", "SL", "Đơn giá", "Thành tiền" };

            g.FillRectangle(brPurple, margin, y, pageW - margin * 2, 28);
            for (int i = 0; i < headers.Length; i++)
                g.DrawString(headers[i], new Font("Arial", 9, FontStyle.Bold),
                    Brushes.White, colX[i] + 4, y + 6);
            y += 28;

            bool alt = false; int stt = 1;

            // Dòng tiền phòng
            DrawTableRow(g, fntSmall, brGray, colX, colW, ref y, ref alt, ref stt,
                $"Tiền phòng ({booking.Nights} đêm)",
                1, booking.RoomAmount / Math.Max(1, booking.Nights), booking.RoomAmount);

            // Dịch vụ
            foreach (var svc in services)
                DrawTableRow(g, fntSmall, brGray, colX, colW, ref y, ref alt, ref stt,
                    svc.ServiceName, svc.Count, svc.UnitPrice, svc.Total);

            g.DrawLine(new Pen(Color.FromArgb(106, 90, 205), 1.5f), margin, y, pageW - margin, y);
            y += 12;

            // Tổng tiền
            float totalX = pageW - margin - 220;
            DrawTotalRow(g, fntNormal, fntBold, totalX, ref y,
                "Tiền phòng:", booking.RoomAmount.ToString("N0") + " ₫");
            DrawTotalRow(g, fntNormal, fntBold, totalX, ref y,
                "Tiền dịch vụ:", booking.ServiceAmount.ToString("N0") + " ₫");

            g.DrawLine(new Pen(Color.FromArgb(106, 90, 205)), totalX, y, pageW - margin, y);
            y += 8;

            g.FillRectangle(brGray, totalX - 10, y - 4, pageW - margin - totalX + 10, 32);
            g.DrawString("TỔNG CỘNG:", fntTotal, brPurple, totalX, y);
            var tsz = g.MeasureString(booking.TotalPrice.ToString("N0") + " ₫", fntTotal);
            g.DrawString(booking.TotalPrice.ToString("N0") + " ₫", fntTotal, brRed,
                pageW - margin - tsz.Width, y);
            y += 45;

            // Trạng thái
            string stText = booking.Status == "Đã trả" ? "✓  ĐÃ THANH TOÁN" : "⏳  CHƯA THANH TOÁN";
            var stBrush = booking.Status == "Đã trả"
                ? new SolidBrush(Color.FromArgb(40, 167, 69))
                : new SolidBrush(Color.FromArgb(255, 140, 0));
            var stSize = g.MeasureString(stText, new Font("Arial", 12, FontStyle.Bold));
            g.DrawString(stText, new Font("Arial", 12, FontStyle.Bold), stBrush,
                (pageW - stSize.Width) / 2, y);
            y += 45;

            // Footer
            g.DrawLine(Pens.LightGray, margin, y, pageW - margin, y);
            y += 10;
            var footer = "Cảm ơn quý khách đã sử dụng dịch vụ! Hẹn gặp lại.";
            var fsz = g.MeasureString(footer, fntSmall);
            g.DrawString(footer, fntSmall, Brushes.Gray, (pageW - fsz.Width) / 2, y);
        }

        private void DrawInfoRow(Graphics g, Font fntLabel, Font fntVal,
            float x, ref float y, string label, string value)
        {
            g.DrawString(label, fntLabel, Brushes.Black, x, y);
            g.DrawString(value, fntVal, Brushes.Black, x + 90, y);
            y += 22;
        }

        private void DrawTableRow(Graphics g, Font fnt, Brush brAlt,
            float[] colX, float[] colW,
            ref float y, ref bool alt, ref int stt,
            string name, int qty, decimal unitPrice, decimal total)
        {
            if (alt) g.FillRectangle(brAlt, colX[0], y,
                colX[colX.Length - 1] + colW[colW.Length - 1] - colX[0], 24);
            g.DrawString(stt.ToString(), fnt, Brushes.Black, colX[0] + 4, y + 3);
            g.DrawString(name, fnt, Brushes.Black, colX[1] + 4, y + 3);
            g.DrawString(qty.ToString(), fnt, Brushes.Black, colX[2] + 4, y + 3);
            g.DrawString(unitPrice.ToString("N0"), fnt, Brushes.Black, colX[3] + 4, y + 3);
            g.DrawString(total.ToString("N0") + " ₫", fnt, Brushes.Black, colX[4] + 4, y + 3);
            y += 24; alt = !alt; stt++;
        }

        private void DrawTotalRow(Graphics g, Font fntLabel, Font fntVal,
            float x, ref float y, string label, string value)
        {
            g.DrawString(label, fntLabel, Brushes.Black, x, y);
            var vsz = g.MeasureString(value, fntVal);
            g.DrawString(value, fntVal, Brushes.Black, x + 210 - vsz.Width, y);
            y += 22;
        }
    }
}
