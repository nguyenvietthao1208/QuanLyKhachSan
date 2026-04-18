// =============================================
// UC - Báo Cáo (ucBaoCao.cs)
// Xuất Excel, PDF, In ấn
// =============================================
// LƯU Ý: Cần cài NuGet packages:
//   - EPPlus (cho Excel)  : Install-Package EPPlus
//   - iTextSharp (cho PDF): Install-Package iTextSharp
// =============================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuanLyKhachSan.BLL;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.GUI
{
    public class ucBaoCao : UserControl
    {
        private Panel        pnlTop;
        private Label        lblTitle;
        private TabControl   tabReport;
        private TabPage      tabTongHop, tabTheoPhong, tabTheoKhach;

        // Bộ lọc thời gian
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnFilter, btnExcelOverall, btnPDFOverall, btnPrint;

        // DataGridViews
        private DataGridView dgvOverall;
        private DataGridView dgvByRoom;
        private DataGridView dgvByCustomer;

        // Summary labels
        private Label lblSumRooms, lblSumRevenue, lblSumServices;

        private readonly BookingBLL _bookingBLL = new BookingBLL();
        private readonly Color PrimaryColor = Color.FromArgb(106, 90, 205);

        private List<BookingDTO> _allData = new List<BookingDTO>();

        public ucBaoCao()
        {
            InitializeComponent();
            LoadAll();
        }

        private void InitializeComponent()
        {
            this.Dock      = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 248, 252);

            // ── HEADER ────────────────────────────────────────
            pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 110,
                BackColor = Color.White,
                Padding   = new Padding(15, 10, 15, 10)
            };

            lblTitle = new Label
            {
                Text      = "📊  Báo Cáo Hệ Thống",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location  = new Point(15, 12),
                AutoSize  = true
            };

            // Bộ lọc
            var lblFrom = new Label { Text = "Từ ngày:", Location = new Point(15, 48), AutoSize = true };
            dtpFrom = new DateTimePicker
            {
                Location = new Point(80, 44),
                Size     = new Size(140, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today.AddMonths(-1)
            };

            var lblTo = new Label { Text = "Đến:", Location = new Point(235, 48), AutoSize = true };
            dtpTo = new DateTimePicker
            {
                Location = new Point(268, 44),
                Size     = new Size(140, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today
            };

            btnFilter = MakeBtn("🔍 Lọc", PrimaryColor, new Point(425, 44), 85);
            btnFilter.Click += (s, e) => ApplyFilter();

            btnExcelOverall = MakeBtn("📥 Excel", Color.FromArgb(33, 115, 70), new Point(520, 44), 90);
            btnExcelOverall.Click += BtnExcel_Click;

            btnPDFOverall = MakeBtn("📄 PDF", Color.FromArgb(200, 50, 50), new Point(618, 44), 80);
            btnPDFOverall.Click += BtnPDF_Click;

            btnPrint = MakeBtn("🖨 In", Color.FromArgb(50, 100, 170), new Point(706, 44), 75);
            btnPrint.Click += BtnPrint_Click;

            // Tóm tắt
            lblSumRooms    = MakeSummaryLabel("Tổng lượt thuê: 0",     new Point(15, 82));
            lblSumRevenue  = MakeSummaryLabel("Doanh thu: 0 ₫",        new Point(200, 82));
            lblSumServices = MakeSummaryLabel("Thu từ dịch vụ: 0 ₫",   new Point(430, 82));

            pnlTop.Controls.AddRange(new Control[]
            {
                lblTitle, lblFrom, dtpFrom, lblTo, dtpTo,
                btnFilter, btnExcelOverall, btnPDFOverall, btnPrint,
                lblSumRooms, lblSumRevenue, lblSumServices
            });

            // ── TABS ──────────────────────────────────────────
            tabReport = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            tabTongHop   = new TabPage("📋  Tổng Hợp");
            tabTheoPhong = new TabPage("🏠  Theo Phòng");
            tabTheoKhach = new TabPage("👥  Theo Khách Hàng");

            tabReport.TabPages.AddRange(new TabPage[] { tabTongHop, tabTheoPhong, tabTheoKhach });

            // Build DGVs
            dgvOverall    = BuildReportDGV();
            dgvByRoom     = BuildReportDGV();
            dgvByCustomer = BuildReportDGV();

            // Cột tab Tổng hợp
            dgvOverall.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Id",       HeaderText="Mã HĐ",       Width=65 },
                new DataGridViewTextBoxColumn { Name="Room",     HeaderText="Phòng",        Width=80 },
                new DataGridViewTextBoxColumn { Name="Customer", HeaderText="Khách hàng",   Width=160 },
                new DataGridViewTextBoxColumn { Name="Phone",    HeaderText="SĐT",           Width=110 },
                new DataGridViewTextBoxColumn { Name="CheckIn",  HeaderText="Check-in",     Width=100 },
                new DataGridViewTextBoxColumn { Name="CheckOut", HeaderText="Check-out",    Width=100 },
                new DataGridViewTextBoxColumn { Name="Nights",   HeaderText="Đêm",          Width=60 },
                new DataGridViewTextBoxColumn { Name="RoomAmt",  HeaderText="Tiền phòng",   Width=110 },
                new DataGridViewTextBoxColumn { Name="SvcAmt",   HeaderText="Tiền DV",      Width=110 },
                new DataGridViewTextBoxColumn { Name="Total",    HeaderText="Tổng",         Width=120, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewTextBoxColumn { Name="Status",   HeaderText="Trạng thái",   Width=100 }
            });

            // Cột tab Theo phòng
            dgvByRoom.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Room",    HeaderText="Phòng",          Width=100 },
                new DataGridViewTextBoxColumn { Name="Count",   HeaderText="Lượt thuê",      Width=100 },
                new DataGridViewTextBoxColumn { Name="Nights",  HeaderText="Tổng đêm",       Width=100 },
                new DataGridViewTextBoxColumn { Name="RoomRev", HeaderText="DT phòng",       Width=140 },
                new DataGridViewTextBoxColumn { Name="SvcRev",  HeaderText="DT dịch vụ",    Width=140 },
                new DataGridViewTextBoxColumn { Name="Total",   HeaderText="Tổng DT",        Width=150, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });

            // Cột tab Theo khách hàng
            dgvByCustomer.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name="Name",   HeaderText="Khách hàng",     Width=180 },
                new DataGridViewTextBoxColumn { Name="Phone",  HeaderText="SĐT",             Width=120 },
                new DataGridViewTextBoxColumn { Name="Count",  HeaderText="Lượt thuê",      Width=100 },
                new DataGridViewTextBoxColumn { Name="Total",  HeaderText="Tổng chi",       Width=150, AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill }
            });

            dgvOverall.Dock    = DockStyle.Fill;
            dgvByRoom.Dock     = DockStyle.Fill;
            dgvByCustomer.Dock = DockStyle.Fill;

            tabTongHop.Controls.Add(dgvOverall);
            tabTheoPhong.Controls.Add(dgvByRoom);
            tabTheoKhach.Controls.Add(dgvByCustomer);

            this.Controls.AddRange(new Control[] { tabReport, pnlTop });
        }

        public void LoadAll()
        {
            _allData = _bookingBLL.GetAll();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _allData
                .Where(b => b.CheckIn.Date >= dtpFrom.Value.Date &&
                            b.CheckIn.Date <= dtpTo.Value.Date)
                .ToList();

            FillOverall(filtered);
            FillByRoom(filtered);
            FillByCustomer(filtered);
            UpdateSummary(filtered);
        }

        private void FillOverall(List<BookingDTO> data)
        {
            dgvOverall.Rows.Clear();
            foreach (var b in data)
            {
                dgvOverall.Rows.Add(
                    b.Id, b.RoomNumber, b.CustomerName, b.CustomerPhone,
                    b.CheckIn.ToString("dd/MM/yyyy"),
                    b.CheckOut.ToString("dd/MM/yyyy"),
                    b.Nights,
                    b.RoomAmount.ToString("N0") + " ₫",
                    b.ServiceAmount.ToString("N0") + " ₫",
                    b.TotalPrice.ToString("N0") + " ₫",
                    b.Status
                );
            }
        }

        private void FillByRoom(List<BookingDTO> data)
        {
            dgvByRoom.Rows.Clear();
            var grouped = data.GroupBy(b => b.RoomNumber)
                              .Select(g => new
                              {
                                  Room    = g.Key,
                                  Count   = g.Count(),
                                  Nights  = g.Sum(b => b.Nights),
                                  RoomRev = g.Sum(b => b.RoomAmount),
                                  SvcRev  = g.Sum(b => b.ServiceAmount),
                                  Total   = g.Sum(b => b.TotalPrice)
                              })
                              .OrderByDescending(x => x.Total);

            foreach (var r in grouped)
                dgvByRoom.Rows.Add(
                    r.Room, r.Count, r.Nights,
                    r.RoomRev.ToString("N0") + " ₫",
                    r.SvcRev.ToString("N0")  + " ₫",
                    r.Total.ToString("N0")   + " ₫"
                );
        }

        private void FillByCustomer(List<BookingDTO> data)
        {
            dgvByCustomer.Rows.Clear();
            var grouped = data.GroupBy(b => new { b.CustomerName, b.CustomerPhone })
                              .Select(g => new
                              {
                                  Name  = g.Key.CustomerName,
                                  Phone = g.Key.CustomerPhone,
                                  Count = g.Count(),
                                  Total = g.Sum(b => b.TotalPrice)
                              })
                              .OrderByDescending(x => x.Total);

            foreach (var c in grouped)
                dgvByCustomer.Rows.Add(c.Name, c.Phone, c.Count, c.Total.ToString("N0") + " ₫");
        }

        private void UpdateSummary(List<BookingDTO> data)
        {
            int count        = data.Count;
            decimal revenue  = data.Sum(b => b.TotalPrice);
            decimal svcTotal = data.Sum(b => b.ServiceAmount);

            lblSumRooms.Text    = $"Tổng lượt thuê: {count}";
            lblSumRevenue.Text  = $"Doanh thu: {revenue:N0} ₫";
            lblSumServices.Text = $"Thu từ dịch vụ: {svcTotal:N0} ₫";
        }

        // ── EXPORT EXCEL ──────────────────────────────────────
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter   = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                Title    = "Lưu báo cáo Excel"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Dùng EPPlus nếu đã cài NuGet EPPlus
                // Nếu chưa cài, tạo file CSV thay thế
                ExportToCSV(dlg.FileName.Replace(".xlsx", ".csv"));
                MessageBox.Show($"Xuất file thành công!\n{dlg.FileName.Replace(".xlsx", ".csv")}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Header
                sw.WriteLine("BÁO CÁO QUẢN LÝ KHÁCH SẠN");
                sw.WriteLine($"Từ: {dtpFrom.Value:dd/MM/yyyy} - Đến: {dtpTo.Value:dd/MM/yyyy}");
                sw.WriteLine($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                sw.WriteLine();
                sw.WriteLine("Mã HĐ,Phòng,Khách hàng,SĐT,Check-in,Check-out,Số đêm,Tiền phòng,Tiền DV,Tổng tiền,Trạng thái");

                foreach (DataGridViewRow row in dgvOverall.Rows)
                {
                    var cells = new string[11];
                    for (int i = 0; i < 11; i++)
                        cells[i] = row.Cells[i].Value?.ToString() ?? "";
                    sw.WriteLine(string.Join(",", cells));
                }

                sw.WriteLine();
                sw.WriteLine($"Tổng lượt thuê,{lblSumRooms.Text}");
                sw.WriteLine($"Doanh thu,{lblSumRevenue.Text}");
            }
        }

        // ── EXPORT PDF ────────────────────────────────────────
        private void BtnPDF_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter   = "PDF files (*.pdf)|*.pdf",
                FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                Title    = "Lưu báo cáo PDF"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            /*
            MessageBox.Show(
                "Để xuất PDF, hãy cài NuGet package:\n\n" +
                "Tools → NuGet Package Manager → Console:\n" +
                "Install-Package iTextSharp\n\n" +
                "Sau đó gọi hàm ExportToPDF(filePath) đã được chuẩn bị sẵn.",
                "Hướng dẫn cài PDF",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            */

            // Khi đã cài iTextSharp, bỏ comment đoạn dưới:
            ExportToPDF(dlg.FileName);
        }

        ///* 
        // ── Uncomment sau khi cài iTextSharp ─────────────────
        private void ExportToPDF(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // Font hỗ trợ Unicode tiếng Việt
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var bfViet = iTextSharp.text.pdf.BaseFont.CreateFont(fontPath,
                    iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                var fontTitle = new iTextSharp.text.Font(bfViet, 16, iTextSharp.text.Font.BOLD);
                var fontBody  = new iTextSharp.text.Font(bfViet, 9);
                var fontHead  = new iTextSharp.text.Font(bfViet, 9, iTextSharp.text.Font.BOLD);

                doc.Add(new iTextSharp.text.Paragraph("BÁO CÁO QUẢN LÝ KHÁCH SẠN", fontTitle));
                doc.Add(new iTextSharp.text.Paragraph($"Từ: {dtpFrom.Value:dd/MM/yyyy} - Đến: {dtpTo.Value:dd/MM/yyyy}", fontBody));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // Table
                var table = new iTextSharp.text.pdf.PdfPTable(11) { WidthPercentage = 100 };
                string[] headers = { "Mã HĐ","Phòng","Khách hàng","SĐT","Check-in","Check-out","Đêm","Tiền phòng","Tiền DV","Tổng","TT" };
                foreach (var h in headers)
                {
                    var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(h, fontHead));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(106, 90, 205);
                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                foreach (DataGridViewRow row in dgvOverall.Rows)
                    for (int i = 0; i < 11; i++)
                        table.AddCell(new iTextSharp.text.Phrase(row.Cells[i].Value?.ToString() ?? "", fontBody));

                doc.Add(table);
                doc.Close();
            }

            MessageBox.Show("Xuất PDF thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start(filePath);
        }
        //*/

        // ── IN ẤN ─────────────────────────────────────────────
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            var pd = new PrintDocument();
            pd.PrintPage += PrintPage_Handler;

            var preview = new PrintPreviewDialog
            {
                Document = pd,
                WindowState = FormWindowState.Maximized
            };
            preview.ShowDialog();
        }

        private void PrintPage_Handler(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var font      = new Font("Arial", 9);
            var fontBold  = new Font("Arial", 10, FontStyle.Bold);
            var fontTitle = new Font("Arial", 14, FontStyle.Bold);
            var brush     = Brushes.Black;

            float y = 40;
            g.DrawString("BÁO CÁO QUẢN LÝ KHÁCH SẠN", fontTitle, brush, 200, y); y += 30;
            g.DrawString($"Từ: {dtpFrom.Value:dd/MM/yyyy}  -  Đến: {dtpTo.Value:dd/MM/yyyy}", font, brush, 200, y); y += 20;
            g.DrawString($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", font, brush, 200, y); y += 30;

            // Summary
            g.DrawString(lblSumRooms.Text,    fontBold, Brushes.DarkBlue, 40, y); y += 20;
            g.DrawString(lblSumRevenue.Text,  fontBold, Brushes.DarkGreen, 40, y); y += 20;
            g.DrawString(lblSumServices.Text, fontBold, Brushes.DarkRed, 40, y); y += 30;

            // Table header
            string[] headers = { "Phòng", "Khách hàng", "Check-in", "Check-out", "Tổng tiền", "TT" };
            int[]    widths  = { 60, 160, 90, 90, 110, 80 };
            float    x = 40;

            var headerBrush = new SolidBrush(Color.FromArgb(106, 90, 205));
            for (int i = 0; i < headers.Length; i++)
            {
                g.FillRectangle(headerBrush, x, y, widths[i], 22);
                g.DrawString(headers[i], fontBold, Brushes.White, x + 3, y + 3);
                x += widths[i];
            }
            y += 24;

            // Rows
            bool alt = false;
            foreach (DataGridViewRow row in dgvOverall.Rows)
            {
                if (y > e.PageBounds.Height - 60) { e.HasMorePages = true; break; }
                x = 40;
                var bg = alt ? new SolidBrush(Color.FromArgb(245, 245, 252)) : Brushes.White;
                g.FillRectangle(bg, 40, y, widths.Sum(), 20);

                int[] colIdx = { 1, 2, 4, 5, 9, 10 };
                for (int i = 0; i < colIdx.Length; i++)
                {
                    string val = row.Cells[colIdx[i]].Value?.ToString() ?? "";
                    g.DrawString(val, font, Brushes.Black, x + 2, y + 3);
                    x += widths[i];
                }
                y += 20;
                alt = !alt;
            }
        }

        // ── HELPERS ───────────────────────────────────────────
        private Button MakeBtn(string text, Color color, Point loc, int w)
        {
            var btn = new Button
            {
                Text = text, Location = loc, Size = new Size(w, 28),
                FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label MakeSummaryLabel(string text, Point loc) =>
            new Label
            {
                Text      = text,
                Location  = loc,
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

        private DataGridView BuildReportDGV() => new DataGridView
        {
            ReadOnly          = true,
            AllowUserToAddRows = false,
            SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor   = Color.White,
            BorderStyle       = BorderStyle.None,
            Font              = new Font("Segoe UI", 9.5f),
            RowHeadersVisible = false,
            ColumnHeadersHeight = 38,
            RowTemplate       = { Height = 32 }
        };
    }
}
