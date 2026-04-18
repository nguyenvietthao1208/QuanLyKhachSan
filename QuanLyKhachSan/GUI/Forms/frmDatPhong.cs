// =============================================
// GUI - Form Đặt Phòng (frmDatPhong.cs)
// =============================================

using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class frmDatPhong : Form
    {
        private RoomDTO _room;
        private readonly BookingBLL  _bookingBLL  = new BookingBLL();
        private readonly CustomerBLL _customerBLL = new CustomerBLL();

        // Controls
        private Panel   pnlHeader;
        private Label   lblTitle;
        private Label   lblRoomDetail;

        private GroupBox grpCustomer;
        private Label   lblName, lblIdCard, lblPhone, lblAddress, lblEmail;
        private TextBox txtName, txtIdCard, txtPhone, txtAddress, txtEmail;
        private CheckBox chkExisting;
        private ComboBox cmbExistingCustomer;

        private GroupBox grpBooking;
        private Label   lblCheckIn, lblCheckOut, lblNote, lblNights, lblPrice;
        private DateTimePicker dtpCheckIn, dtpCheckOut;
        private TextBox txtNote;
        private Label   lblNightsValue, lblPriceValue;

        private Button btnSave, btnCancel;

        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        public frmDatPhong(RoomDTO room)
        {
            _room = room;
            InitializeComponent();
            LoadExistingCustomers();
            UpdatePricePreview();
        }

        private void InitializeComponent()
        {
            this.Text            = $"Đặt phòng {_room.RoomNumber}";
            this.Size            = new Size(540, 620);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.BackColor       = Color.FromArgb(248, 248, 252);
            this.Font            = new Font("Segoe UI", 9.5f);

            // Header
            pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = PrimaryColor
            };

            lblTitle = new Label
            {
                Text      = $"📋  Đặt Phòng {_room.RoomNumber}",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(20, 12),
                AutoSize  = true
            };

            lblRoomDetail = new Label
            {
                Text      = $"Loại: {_room.CategoryName}  |  Giá: {_room.CategoryPrice:N0} ₫/đêm",
                Font      = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(210, 210, 255),
                Location  = new Point(20, 44),
                AutoSize  = true
            };

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblRoomDetail });

            // ── KHÁCH HÀNG ────────────────────────────────────
            grpCustomer = new GroupBox
            {
                Text     = "Thông Tin Khách Hàng",
                Location = new Point(15, 85),
                Size     = new Size(500, 250),
                Font     = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            chkExisting = new CheckBox
            {
                Text     = "Khách hàng đã có trong hệ thống",
                Location = new Point(12, 25),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9)
            };
            chkExisting.CheckedChanged += ChkExisting_CheckedChanged;

            cmbExistingCustomer = new ComboBox
            {
                Location      = new Point(12, 50),
                Size          = new Size(470, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9),
                Visible       = false
            };
            cmbExistingCustomer.SelectedIndexChanged += (s, e) => FillFromExisting();

            // Fields
            int y = 52;
            lblName   = MakeLabel("Họ và tên *",      12,  y);      txtName   = MakeTxt(120, y, 362);    y += 40;
            lblIdCard = MakeLabel("CCCD/CMND *",      12,  y);      txtIdCard = MakeTxt(120, y, 185);    y += 40;
            lblPhone  = MakeLabel("Số điện thoại *",  12,  y);      txtPhone  = MakeTxt(120, y, 185);    y += 40;
            lblAddress= MakeLabel("Địa chỉ",          12,  y);      txtAddress= MakeTxt(120, y, 362);    y += 40;
            lblEmail  = MakeLabel("Email",             12,  y);      txtEmail  = MakeTxt(120, y, 362);

            grpCustomer.Controls.AddRange(new Control[]
            {
                chkExisting, cmbExistingCustomer,
                lblName, txtName, lblIdCard, txtIdCard, lblPhone, txtPhone,
                lblAddress, txtAddress, lblEmail, txtEmail
            });

            // ── BOOKING ───────────────────────────────────────
            grpBooking = new GroupBox
            {
                Text      = "Thông Tin Đặt Phòng",
                Location  = new Point(15, 348),
                Size      = new Size(500, 180),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            lblCheckIn = MakeLabel("Ngày nhận:", 12, 28);
            dtpCheckIn = new DateTimePicker
            {
                Location = new Point(110, 25),
                Size     = new Size(160, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today
            };
            dtpCheckIn.ValueChanged += (s, e) => UpdatePricePreview();

            lblCheckOut = MakeLabel("Ngày trả:", 295, 28);
            dtpCheckOut = new DateTimePicker
            {
                Location = new Point(370, 25),
                Size     = new Size(120, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today.AddDays(1)
            };
            dtpCheckOut.ValueChanged += (s, e) => UpdatePricePreview();

            lblNights = MakeLabel("Số đêm:", 12, 68);
            lblNightsValue = new Label
            {
                Location  = new Point(110, 68),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Text      = "1 đêm"
            };

            lblPrice = MakeLabel("Tiền phòng:", 295, 68);
            lblPriceValue = new Label
            {
                Location  = new Point(370, 68),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Text      = "0 ₫"
            };

            lblNote = MakeLabel("Ghi chú:", 12, 108);
            txtNote = new TextBox
            {
                Location  = new Point(110, 105),
                Size      = new Size(380, 28),
                Font      = new Font("Segoe UI", 9),
                MaxLength = 500
            };

            grpBooking.Controls.AddRange(new Control[]
            {
                lblCheckIn, dtpCheckIn, lblCheckOut, dtpCheckOut,
                lblNights, lblNightsValue, lblPrice, lblPriceValue,
                lblNote, txtNote
            });

            // ── BUTTONS ───────────────────────────────────────
            btnSave = new Button
            {
                Text      = "✔  Xác Nhận Đặt Phòng",
                Location  = new Point(15, 542),
                Size      = new Size(240, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text      = "✖  Hủy",
                Location  = new Point(270, 542),
                Size      = new Size(120, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11),
                Cursor    = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[]
            {
                pnlHeader, grpCustomer, grpBooking, btnSave, btnCancel
            });
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text      = text,
                Location  = new Point(x, y + 4),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
        }

        private TextBox MakeTxt(int x, int y, int width)
        {
            return new TextBox
            {
                Location  = new Point(x, y),
                Size      = new Size(width, 28),
                Font      = new Font("Segoe UI", 9),
                BackColor = Color.White
            };
        }

        private void LoadExistingCustomers()
        {
            var list = _customerBLL.GetAll();
            cmbExistingCustomer.DataSource    = list;
            cmbExistingCustomer.DisplayMember = "FullName";
            cmbExistingCustomer.ValueMember   = "Id";
        }

        private void ChkExisting_CheckedChanged(object sender, EventArgs e)
        {
            bool isExisting = chkExisting.Checked;
            cmbExistingCustomer.Visible = isExisting;

            // Shift fields down khi chọn có sẵn
            int shift = isExisting ? 30 : -30;
            foreach (Control c in grpCustomer.Controls)
            {
                if (c != chkExisting && c != cmbExistingCustomer)
                    c.Top += shift;
            }

            // Disable/enable inputs
            txtName.ReadOnly    = isExisting;
            txtIdCard.ReadOnly  = isExisting;
            txtPhone.ReadOnly   = isExisting;
            txtAddress.ReadOnly = isExisting;
            txtEmail.ReadOnly   = isExisting;

            if (isExisting) FillFromExisting();
            else { txtName.Text = txtIdCard.Text = txtPhone.Text = txtAddress.Text = txtEmail.Text = ""; }
        }

        private void FillFromExisting()
        {
            if (cmbExistingCustomer.SelectedItem is CustomerDTO cust)
            {
                txtName.Text    = cust.FullName;
                txtIdCard.Text  = cust.IdCard;
                txtPhone.Text   = cust.Phone;
                txtAddress.Text = cust.Address;
                txtEmail.Text   = cust.Email;
            }
        }

        private void UpdatePricePreview()
        {
            int nights = Math.Max(1, (dtpCheckOut.Value.Date - dtpCheckIn.Value.Date).Days);
            decimal total = nights * _room.CategoryPrice;
            lblNightsValue.Text = $"{nights} đêm";
            lblPriceValue.Text  = $"{total:N0} ₫";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Build customer
            CustomerDTO customer;
            bool isNew;

            if (chkExisting.Checked && cmbExistingCustomer.SelectedItem is CustomerDTO existing)
            {
                customer = existing;
                isNew    = false;
            }
            else
            {
                customer = new CustomerDTO
                {
                    FullName = txtName.Text.Trim(),
                    IdCard   = txtIdCard.Text.Trim(),
                    Phone    = txtPhone.Text.Trim(),
                    Address  = txtAddress.Text.Trim(),
                    Email    = txtEmail.Text.Trim()
                };
                isNew = true;
            }

            // Build booking
            var booking = new BookingDTO
            {
                IdRoom   = _room.Id,
                CheckIn  = dtpCheckIn.Value.Date,
                CheckOut = dtpCheckOut.Value.Date,
                Note     = txtNote.Text.Trim()
            };

            var (ok, msg) = _bookingBLL.BookRoom(customer, booking, isNew);

            MessageBox.Show(msg, ok ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok) this.DialogResult = DialogResult.OK;
        }
    }
}
