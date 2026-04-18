using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class frmProfile : Form
    {
        private AccountDTO _account;
        private readonly AccountBLL _bll = new AccountBLL();
        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        private Label lblUsername, lblRole;
        private TextBox txtDisplayName;
        private TextBox txtOldPass, txtNewPass, txtConfirmPass;
        private Button btnSaveInfo, btnChangePass, btnClose;
        private GroupBox grpInfo, grpPassword;

        public frmProfile(AccountDTO account)
        {
            _account = account;
            BuildUI();
            FillInfo();
        }

        private void BuildUI()
        {
            this.Text = "Thông Tin Cá Nhân";
            this.Size = new Size(430, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 248, 252);
            this.Font = new Font("Segoe UI", 9.5f);

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = PrimaryColor };
            pnlHeader.Controls.Add(new Label
            {
                Text = "👤  Thông Tin Tài Khoản",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 18)
            });

            // ── GROUP THÔNG TIN ───────────────────────────────
            grpInfo = new GroupBox
            {
                Text = "Thông tin cơ bản",
                Location = new Point(15, 80),
                Size = new Size(385, 165),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            grpInfo.Controls.Add(MakeLbl("Tên đăng nhập:", 12, 30));
            lblUsername = new Label
            {
                Location = new Point(150, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            grpInfo.Controls.Add(lblUsername);

            grpInfo.Controls.Add(MakeLbl("Quyền hạn:", 12, 62));
            lblRole = new Label
            {
                Location = new Point(150, 62),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            grpInfo.Controls.Add(lblRole);

            grpInfo.Controls.Add(MakeLbl("Họ và tên:", 12, 97));
            txtDisplayName = new TextBox
            {
                Location = new Point(150, 94),
                Size = new Size(220, 28),
                Font = new Font("Segoe UI", 9.5f)
            };
            grpInfo.Controls.Add(txtDisplayName);

            btnSaveInfo = MakeBtn("💾  Lưu thông tin", PrimaryColor, new Point(12, 130), 175);
            btnSaveInfo.Click += BtnSaveInfo_Click;
            grpInfo.Controls.Add(btnSaveInfo);

            // ── GROUP ĐỔI MẬT KHẨU ───────────────────────────
            grpPassword = new GroupBox
            {
                Text = "Đổi mật khẩu",
                Location = new Point(15, 258),
                Size = new Size(385, 180),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            grpPassword.Controls.Add(MakeLbl("Mật khẩu cũ:", 12, 30));
            txtOldPass = MakePassBox(150, 27, 220);
            grpPassword.Controls.Add(txtOldPass);

            grpPassword.Controls.Add(MakeLbl("Mật khẩu mới:", 12, 68));
            txtNewPass = MakePassBox(150, 65, 220);
            grpPassword.Controls.Add(txtNewPass);

            grpPassword.Controls.Add(MakeLbl("Xác nhận lại:", 12, 106));
            txtConfirmPass = MakePassBox(150, 103, 220);
            grpPassword.Controls.Add(txtConfirmPass);

            btnChangePass = MakeBtn("🔑  Đổi mật khẩu",
                Color.FromArgb(23, 162, 184), new Point(12, 138), 175);
            btnChangePass.Click += BtnChangePass_Click;
            grpPassword.Controls.Add(btnChangePass);

            // Nút đóng
            btnClose = MakeBtn("✖  Đóng",
                Color.FromArgb(108, 117, 125), new Point(295, 450), 120);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
                { pnlHeader, grpInfo, grpPassword, btnClose });
        }

        private void FillInfo()
        {
            lblUsername.Text = _account.UserName;
            lblRole.Text = _account.IsAdmin ? "ADMIN" : "NHÂN VIÊN";
            lblRole.ForeColor = _account.IsAdmin
                ? Color.FromArgb(40, 167, 69)
                : Color.FromArgb(23, 162, 184);
            txtDisplayName.Text = _account.DisplayName;
        }

        private void BtnSaveInfo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
            {
                MessageBox.Show("Họ tên không được để trống!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updated = new AccountDTO
            {
                Id = _account.Id,
                UserName = _account.UserName,
                DisplayName = txtDisplayName.Text.Trim(),
                PassWord = _account.PassWord,
                Role = _account.Role
            };

            var (ok, msg) = _bll.Update(updated);
            if (ok)
            {
                _account.DisplayName = updated.DisplayName;
                MessageBox.Show("Cập nhật thông tin thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnChangePass_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOldPass.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu cũ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtOldPass.Text != _account.PassWord)
            {
                MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOldPass.Clear(); txtOldPass.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPass.Text) || txtNewPass.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải từ 6 ký tự trở lên!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPass.Clear(); txtConfirmPass.Focus();
                return;
            }

            var updated = new AccountDTO
            {
                Id = _account.Id,
                UserName = _account.UserName,
                DisplayName = _account.DisplayName,
                PassWord = txtNewPass.Text,
                Role = _account.Role
            };

            var (ok, msg) = _bll.Update(updated);
            if (ok)
            {
                _account.PassWord = txtNewPass.Text;
                txtOldPass.Clear(); txtNewPass.Clear(); txtConfirmPass.Clear();
                MessageBox.Show("Đổi mật khẩu thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── Helpers ───────────────────────────────────────────
        private Label MakeLbl(string text, int x, int y) =>
            new Label
            {
                Text = text,
                Location = new Point(x, y + 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(70, 70, 70)
            };

        private TextBox MakePassBox(int x, int y, int w) =>
            new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 28),
                Font = new Font("Segoe UI", 9.5f),
                UseSystemPasswordChar = true
            };

        private Button MakeBtn(string text, Color color, Point loc, int w)
        {
            var btn = new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(w, 33),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}